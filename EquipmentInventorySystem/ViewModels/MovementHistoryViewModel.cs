using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using EquipmentInventorySystem.Data.Repositories;
using EquipmentInventorySystem.Models;

namespace EquipmentInventorySystem.ViewModels;

public class MovementHistoryViewModel : INotifyPropertyChanged
{
    private readonly ObservableCollection<MovementHistoryDisplayRow> _allItems = [];
    private string _filterText = string.Empty;

    public ICollectionView Items { get; }

    public string FilterText
    {
        get => _filterText;
        set
        {
            _filterText = value;
            OnPropertyChanged(nameof(FilterText));
            Items.Refresh();
        }
    }

    public MovementHistoryViewModel()
    {
        Items = CollectionViewSource.GetDefaultView(_allItems);
        Items.Filter = ApplyFilter;
    }

    public void LoadItems()
    {
        _allItems.Clear();
        foreach (var row in new MovementHistoryRepository().GetAllWithDetails())
            _allItems.Add(row);
    }

    private bool ApplyFilter(object obj)
    {
        if (string.IsNullOrWhiteSpace(_filterText)) return true;
        if (obj is not MovementHistoryDisplayRow row) return false;
        var text = _filterText.Trim().ToLowerInvariant();
        return row.EquipmentName.ToLowerInvariant().Contains(text)
            || row.InventoryNumber.ToLowerInvariant().Contains(text);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
