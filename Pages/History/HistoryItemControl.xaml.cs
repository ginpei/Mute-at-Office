using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using System;

namespace Mute_at_Office.Pages.History
{
    public sealed partial class HistoryItemControl : UserControl
    {
        public static readonly DependencyProperty DateTimeTextProperty =
            DependencyProperty.Register(
                nameof(DateTimeText),
                typeof(string),
                typeof(HistoryItemControl),
                new PropertyMetadata(string.Empty, OnPropertyChanged));

        public static readonly DependencyProperty EventTypeProperty =
            DependencyProperty.Register(
                nameof(EventType),
                typeof(string),
                typeof(HistoryItemControl),
                new PropertyMetadata(string.Empty, OnPropertyChanged));

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                nameof(Message),
                typeof(string),
                typeof(HistoryItemControl),
                new PropertyMetadata(string.Empty, OnPropertyChanged));

        public HistoryItemControl()
        {
            InitializeComponent();
            Loaded += HistoryItemControl_Loaded;
            Unloaded += HistoryItemControl_Unloaded;
        }

        public string DateTimeText
        {
            get => (string)GetValue(DateTimeTextProperty);
            set => SetValue(DateTimeTextProperty, value);
        }

        public string EventType
        {
            get => (string)GetValue(EventTypeProperty);
            set => SetValue(EventTypeProperty, value);
        }

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        private void HistoryItemControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDisplay();
        }

        private void HistoryItemControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= HistoryItemControl_Loaded;
            Unloaded -= HistoryItemControl_Unloaded;
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HistoryItemControl control && control.IsLoaded)
            {
                control.UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            if (HistoryTextBlock == null)
            {
                return;
            }

            HistoryTextBlock.Inlines.Clear();
            
            var dateTimeRun = new Run
            {
                Text = $"{DateTimeText} - [{EventType}] ",
                FontSize = 10,
                FontWeight = Microsoft.UI.Text.FontWeights.Light
            };
            var messageRun = new Run { Text = Message };
            
            HistoryTextBlock.Inlines.Add(dateTimeRun);
            HistoryTextBlock.Inlines.Add(messageRun);
        }
    }
}
