using System;
using System.Windows;
using System.Windows.Controls;
using Fasetto.Word.Core;

namespace Fasetto.Word
{
    /// <summary>
    /// Логика взаимодействия для PasswordEntryControl.xaml
    /// </summary>
    public partial class PasswordEntryControl : UserControl
    {
        #region Dependency Properties

        /// <summary>
        /// The label width of the control
        /// </summary>
        public GridLength LabelWidth
        {
            get => (GridLength)GetValue(LabelWidthProperty);
            set => SetValue(LabelWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for LabelWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(GridLength), typeof(PasswordEntryControl), new PropertyMetadata(GridLength.Auto, LabelWidthChangedCallback));

        #endregion

        #region Constructor

        public PasswordEntryControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Dependency Callbacks

        /// <summary>
        /// Called when the label width has changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void LabelWidthChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //Set the column defenition width to the new value
                (d as PasswordEntryControl).LabelColumnDefenition.Width = (GridLength)e.NewValue;
            }
            catch(Exception ex)
            {
                (d as PasswordEntryControl).LabelColumnDefenition.Width = GridLength.Auto;
            }
        }

        #endregion


        /// <summary>
        /// Update the view model with the new password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Updateview model
            if (DataContext is PasswordEntryViewModel viewModel)
                viewModel.CurrentPassword = CurrentPassword.SecurePassword;
        }

        /// <summary>
        /// Update the view model with the new password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Updateview model
            if (DataContext is PasswordEntryViewModel viewModel)
                viewModel.NewPassword = NewPassword.SecurePassword;
        }

        /// <summary>
        /// Update the view model with the new password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Updateview model
            if (DataContext is PasswordEntryViewModel viewModel)
                viewModel.ConfirmPassword = ConfirmPassword.SecurePassword;
        }
    }
}
