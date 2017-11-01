﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace EditModule.Commands
{
    public class OpenCommandHandler : IEditCommandHandler
    {
        private readonly IOpenSaveActions _openSaveActions;
        private readonly INotify _notify;
        private readonly IStrings _strings;
        private TextEditor _textEditor;

        public OpenCommandHandler(IOpenSaveActions openSaveActions, INotify notify, IStrings strings)
        {
            _openSaveActions = openSaveActions;
            _notify = notify;
            _strings = strings;
        }

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            uiElement.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Execute));
            _textEditor = viewModel.TextEditor;
        }

        public async void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            if (_textEditor.IsModified)
            {
                var result = await _notify.ConfirmYesNoCancel(_strings.SaveYourChanges);
                if (result == MessageBoxResult.Cancel) return;
                if (result == MessageBoxResult.Yes)
                {
                    ApplicationCommands.Save.Execute(null, null);
                    if (_textEditor.IsModified) return;
                }
            }
            Open(ea);
        }

        private async void Open(ExecutedRoutedEventArgs ea)
        {
            var file = ea.Parameter as string ?? _openSaveActions.OpenDialog();
            if (string.IsNullOrEmpty(file)) return;

            try
            {
                var text = Read(file);
                _textEditor.Document.Text = text;
                _textEditor.Document.FileName = file;
                _textEditor.ScrollToHome();
                _textEditor.SelectionStart = 0;
                _textEditor.Document.UndoStack.ClearAll();
                _textEditor.Focus();
                _textEditor.IsModified = false;
            }
            catch (Exception ex)
            {
                await _notify.Alert(ex.Message);
            }
        }

        private string Read(string file)
        {
            var pathExtension = Path.GetExtension(file) ?? throw new ArgumentException(_strings.NoFileExtensionFound, file);

            var isWordDoc = pathExtension.Equals(".docx", StringComparison.OrdinalIgnoreCase);

            var isHtmlFile =
                pathExtension.Equals(".htm", StringComparison.OrdinalIgnoreCase) ||
                pathExtension.Equals(".html", StringComparison.OrdinalIgnoreCase);

            if (isHtmlFile) return _openSaveActions.FromHtml(file);
            if (isWordDoc) return _openSaveActions.FromMicrosoftWord(file);
            return _openSaveActions.Open(file);
        }
    }
}
