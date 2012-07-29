/*******************************************************
 * 
 * ���ߣ�http://www.codeproject.com/Articles/25445/WPF-Command-Pattern-Applied
 * ����ʱ�䣺�ܽ�� 2009
 * ˵�������ļ�ֻ����һ���࣬�������ݼ�����ע�͡�
 * ���л�����.NET 4.0
 * �汾�ţ�1.0.0
 * 
 * ��ʷ��¼��
 * �����ļ� �ܽ�� 2009
 * �������� ����� 20120518
 * 
*******************************************************/

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Controls;
using System.ComponentModel;
using OEA.WPF.Command;
using System.Windows.Automation;
using OEA.Module.WPF;

namespace OEA.WPF.Command
{
    public class MenuItemCommand
    {
        #region CommandProperty

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command", typeof(ICommand), typeof(MenuItemCommand),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCommandChanged)));

        public static ICommand GetCommand(DependencyObject sender)
        {
            return sender.GetValue(CommandProperty) as ICommand;
        }

        public static void SetCommand(DependencyObject sender, ICommand command)
        {
            sender.SetValue(CommandProperty, command);
        }

        #endregion

        static MenuItemCommand()
        {
            UseCommandImage = true;
            ToolTipStyle = CommandToolTipStyle.None;
        }

        public static bool UseCommandImage { get; set; }

        public static CommandToolTipStyle ToolTipStyle { get; set; }

        private static void OnCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            MenuItem menuItem = dependencyObject as MenuItem;
            if (menuItem == null) { throw new InvalidOperationException("menu item required"); }

            var command = e.NewValue as UICommand;
            if (command == null) { throw new InvalidOperationException("CommandAdapter required"); }

            menuItem.Name = "mi" + command.CoreCommand.ProgramingName;
            menuItem.Command = command;
            menuItem.DataContext = command.CoreCommand;
            menuItem.SetBinding(ContentControl.ContentProperty, "Label");
            AutomationProperties.SetName(menuItem, command.CoreCommand.Label);

            SetupImage(menuItem, command);

            CommandToolTipService.SetupTooltip(menuItem, command, ToolTipStyle);
        }

        private static void SetupImage(MenuItem menuItem, UICommand command)
        {
            if (UseCommandImage && menuItem.Icon == null && command != null)
            {
                Uri imageUri = CommandImageService.GetCommandImageUri(command);
                if (imageUri != null)
                {
                    try
                    {
                        menuItem.Icon = new Image
                        {
                            Stretch = Stretch.None,
                            Source = new BitmapImage(imageUri)
                        };
                    }
                    catch { }
                }
            }
        }
    }
}