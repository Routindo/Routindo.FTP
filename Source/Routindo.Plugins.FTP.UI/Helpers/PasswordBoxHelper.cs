using System.Windows;
using System.Windows.Controls;

namespace Routindo.Plugins.FTP.UI.Helpers
{
    public static class PasswordBoxHelper
    {
        #region Static Fields

        /// <summary>
        ///     The bind password.
        /// </summary>
        public static readonly DependencyProperty BindPassword = DependencyProperty.RegisterAttached(
            "BindPassword",
            typeof(bool),
            typeof(PasswordBoxHelper),
            new PropertyMetadata(false, OnBindPasswordChanged));

        /// <summary>
        ///     The bound password.
        /// </summary>
        public static readonly DependencyProperty BoundPassword = DependencyProperty.RegisterAttached(
            "BoundPassword",
            typeof(string),
            typeof(PasswordBoxHelper),
            new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        /// <summary>
        ///     The updating password.
        /// </summary>
        private static readonly DependencyProperty UpdatingPassword =
            DependencyProperty.RegisterAttached(
                "UpdatingPassword",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false));

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get bind password.
        /// </summary>
        /// <param name="dp">
        /// The Dependency Object.
        /// </param>
        /// <returns>
        /// True or false.
        /// </returns>
        public static bool GetBindPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(BindPassword);
        }

        /// <summary>
        /// The get bound password.
        /// </summary>
        /// <param name="dp">
        /// The Dependency Object.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetBoundPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(BoundPassword);
        }

        /// <summary>
        /// The set bind password.
        /// </summary>
        /// <param name="dp">
        /// The Dependency Object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetBindPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(BindPassword, value);
        }

        /// <summary>
        /// The set bound password.
        /// </summary>
        /// <param name="dp">
        /// The Dependency Object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetBoundPassword(DependencyObject dp, string value)
        {
            dp.SetValue(BoundPassword, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get updating password.
        /// </summary>
        /// <param name="dp">
        /// The Dependency Object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool GetUpdatingPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(UpdatingPassword);
        }

        /// <summary>
        /// The handle password changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The Routed Event Args
        /// </param>
        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            var box = sender as PasswordBox;

            // set a flag to indicate that we're updating the password
            SetUpdatingPassword(box, true);

            // push the new password into the BoundPassword property
            if (box == null)
            {
                return;
            }

            SetBoundPassword(box, box.Password);
            SetUpdatingPassword(box, false);
        }

        /// <summary>
        /// The on bind password changed.
        /// </summary>
        /// <param name="dp">
        /// The Dependency Object.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            // when the BindPassword attached property is set on a PasswordBox,
            // start listening to its PasswordChanged event
            var box = dp as PasswordBox;

            if (box == null)
            {
                return;
            }

            var wasBound = (bool)e.OldValue;
            var needToBind = (bool)e.NewValue;

            if (wasBound)
            {
                box.PasswordChanged -= HandlePasswordChanged;
            }

            if (needToBind)
            {
                box.PasswordChanged += HandlePasswordChanged;
            }
        }

        /// <summary>
        /// The on bound password changed.
        /// </summary>
        /// <param name="d">
        /// The Dependency Object.
        /// </param>
        /// <param name="e">
        /// The Dependency object changed event Arguments
        /// </param>
        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as PasswordBox;

            // only handle this event when the property is attached to a PasswordBox
            // and when the BindPassword attached property has been set to true
            if (d == null || !GetBindPassword(d))
            {
                return;
            }

            // avoid recursive updating by ignoring the box's changed event
            // Debug.Assert(box != null, "box != null");
            if (box != null)
            {
                box.PasswordChanged -= HandlePasswordChanged;
            }

            var newPassword = (string)e.NewValue;

            if (!GetUpdatingPassword(box))
            {
                if (box != null)
                {
                    box.Password = newPassword;
                }
            }

            if (box != null)
            {
                box.PasswordChanged += HandlePasswordChanged;
            }
        }

        /// <summary>
        /// The set updating password.
        /// </summary>
        /// <param name="dp">
        /// The Dependency Object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private static void SetUpdatingPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(UpdatingPassword, value);
        }

        #endregion
    }
}
