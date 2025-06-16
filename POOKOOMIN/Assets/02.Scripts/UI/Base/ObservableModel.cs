using System;

public class ObservableModel<T> : INotifyPropertyChanged<T>
{
    public T Value
    {
        get => _value;
        set
        {
            if (_value.Equals(value))
                return;

            _value = value;
            PropertyChanged?.Invoke(this, _value);
        }
    }

    private T _value;

    public event Action<object, T> PropertyChanged;
}

interface INotifyPropertyChanged<T>
{
    event Action<object, T> PropertyChanged;
}