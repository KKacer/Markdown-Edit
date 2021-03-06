﻿using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class ToggleOverTypeModeCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static ToggleOverTypeModeCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute, CanExecute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var editor = ((MainWindow)sender).Editor;
            editor.EditBox.TextArea.OverstrikeMode = !editor.EditBox.TextArea.OverstrikeMode;
        }

        private static void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var editor = ((MainWindow)sender).Editor;
            e.CanExecute = !editor.EditBox.IsReadOnly;
        }
    }
}