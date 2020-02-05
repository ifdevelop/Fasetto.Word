using System;
using System.Windows;

namespace Fasetto.Word
{
    /// <summary>
    /// A base attached propery to replace vanilla WPF attached property
    /// </summary>
    /// <typeparam name="Parent"> The parent class to be attached property</typeparam>
    /// <typeparam name="Property" The type of this attached property </typeparam>
    public abstract class BaseAttachedProperty<Parent, Property>
        where Parent : BaseAttachedProperty<Parent, Property>, new()
    {
        #region Public Events

        /// <summary>
        /// Fired when the value changes
        /// </summary>
        public event Action<DependencyObject, DependencyPropertyChangedEventArgs> ValueChanged = (sender, e) => { };

        /// <summary>
        /// Fired when the value changes, even when the value is the same
        /// </summary>
        public event Action<DependencyObject, object> ValueUpdated = (sender, value) => { };

        #endregion

        #region Public Properties

        /// <summary>
        /// A singleton instance of our parent class
        /// </summary>
        public static Parent Instance { get; private set; } = new Parent();

        #endregion

        #region Attached Property Definitions

        /// <summary>
        /// The attached property for this class
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
            "Value", 
            typeof(Property), 
            typeof(BaseAttachedProperty<Parent, Property>), 
            new UIPropertyMetadata(
                default(Property),
                new PropertyChangedCallback(OnValuePropertyChanged),
                new CoerceValueCallback(OnValuePropertyUpdated)
            ));

        /// <summary>
        /// The callback event when the <see cref="ValueProperty"/> is changed
        /// </summary>
        /// <param name="d"> The UI element that had it's property changed </param>
        /// <param name="e"> The arguments for the events </param>
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Call the paarent function
            Instance.OnValueChanged(d, e);

            // Call event listeners
            Instance.ValueChanged(d, e);
        }


        /// <summary>
        /// The callback event when the <see cref="ValueProperty"/> is changed, even if it is a same value
        /// </summary>
        /// <param name="d"> The UI element that had it's property changed </param>
        /// <param name="value"> Object </param>
        private static object OnValuePropertyUpdated(DependencyObject d, object value)
        {
            // Call the paarent function
            Instance.OnValueUpdated(d, value);

            // Call event listeners
            Instance.ValueUpdated(d, value);

            // Return the value
            return value;
        }


        /// <summary>
        /// Gets the attached property
        /// </summary>
        /// <param name="d"> The element to get property from </param>
        /// <returns></returns>
        public static Property GetValue(DependencyObject d) => (Property)d.GetValue(ValueProperty);

        /// <summary>
        /// Set the attached property
        /// </summary>
        /// <param name="d"> The element to get property from </param>
        /// <param name="value"> The value to set property to </param>
        public static void SetValue(DependencyObject d, Property value) => d.SetValue(ValueProperty, value);

        #endregion

        #region Event Methods

        /// <summary>
        /// The method that is called when any attached property of this type is changed
        /// </summary>
        /// <param name="sender"> The UI element that this was changed for </param>
        /// <param name="e"> The arguments for this event </param>
        public virtual void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }

        /// <summary>
        /// The method that is called when any attached property of this type is changed, even if the value is the same
        /// </summary>
        /// <param name="sender"> The UI element that this was changed for </param>
        /// <param name="value"> Object </param>
        public virtual void OnValueUpdated(DependencyObject sender, object value) { }

        #endregion
    }
}
