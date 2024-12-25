using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Rise.Data.ViewModels
{
    /// <summary>
    /// Base ViewModel implementation, wraps a <typeparamref name="Type"/>
    /// object for the Model-View-ViewModel pattern and contains methods
    /// to handle property changes.
    /// </summary>
    /// <typeparam name="Type">Type of the underlying model.</typeparam>
    public abstract class ViewModel<Type> : ViewModel, IEquatable<Type>, IEquatable<ViewModel<Type>>
    {
        private Type _model;
        /// <summary>
        /// Gets or sets the underlying <see cref="Type"/> object.
        /// </summary>
        public Type Model
        {
            get => _model;
            set
            {
                if (_model == null || !_model.Equals(value))
                {
                    _model = value;

                    // Raise the PropertyChanged event for all properties.
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        public bool Equals(Type other)
            => other.Equals(_model);

        public bool Equals(ViewModel<Type> other)
            => _model.Equals(other._model);
    }

    /// <summary>
    /// Base ViewModel implementation, contains methods to
    /// handle property changes.
    /// </summary>
    public abstract class ViewModel : INotifyPropertyChanged
    {
        private readonly Dictionary<PropertyChangedEventHandler, SynchronizationContext> PropertyChangedEvents =
            new Dictionary<PropertyChangedEventHandler, SynchronizationContext>();

        /// <summary>
        /// Whether or not to send property change notifications.
        /// </summary>
        protected bool NotifyPropertyChanges = true;

        /// <summary> 
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                PropertyChangedEvents.Add(value, SynchronizationContext.Current);
            }
            remove
            {
                PropertyChangedEvents.Remove(value);
            }
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (!NotifyPropertyChanges)
            {
                return;
            }

            PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
            foreach (KeyValuePair<PropertyChangedEventHandler, SynchronizationContext> @event in PropertyChangedEvents)
            {
                if (@event.Value == null)
                {
                    @event.Key.Invoke(this, args);
                }
                else
                {
                    @event.Value.Post(s => @event.Key.Invoke(s, args), this);
                }
            }
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected bool Set<T>(ref T storage, T value,
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;

            if (NotifyPropertyChanges)
            {
                OnPropertyChanged(propertyName);
            }

            return true;
        }
    }
}
