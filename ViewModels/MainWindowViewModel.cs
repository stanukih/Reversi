using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReversiOthello.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
        
    private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public ObservableCollection<field> fields { get; set; }

    public MainWindowViewModel()
    {
        initData();
    }
    

    public void initData()
    {
        fields = new ObservableCollection<field>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if ((i == 3 && j == 3)||(i == 4 && j == 4))
                {
                    fields.Add(new field(
                        initFieldStatus:fieldStatus.WhiteField, 
                        initColumn:j, 
                        initRow:i,
                        initDataContext:this));
                    fields[fields.Count-1].update();
                    continue;
                }
                if ((i == 3 && j == 4)||(i == 4 && j == 3))
                {
                    fields.Add(new field(
                        initFieldStatus:fieldStatus.BlackField, 
                        initColumn:j, 
                        initRow:i,
                        initDataContext:this));
                    fields[fields.Count-1].update();
                    continue;
                }
                fields.Add(new field(
                    initFieldStatus:fieldStatus.FreeField, 
                    initColumn:j, 
                    initRow:i,
                    initDataContext:this));
                fields[fields.Count-1].update();
            }
        }

        GC.Collect(0);
        RaisePropertyChanged(nameof(fields));
        enabledMove();
    }

    public fieldStatus activePlayer
    {
        get;
        set;
    } = fieldStatus.BlackField;

    public void swipePlayer()
    {
        if (activePlayer == fieldStatus.BlackField) activePlayer = fieldStatus.WhiteField;
        else activePlayer = fieldStatus.BlackField;
    }

    public void enabledMove()
    {
        foreach (var VARIABLE in fields)
        {
            Console.WriteLine($"rov {VARIABLE.row} col {VARIABLE.column}");
        }
    }
}






public class field: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public int row { get; set; }
    public int column { get; set; }
    
    private MainWindowViewModel DataMainWindowViewModel { get; set; }
    public field(
        fieldStatus initFieldStatus,
        int initColumn,
        int initRow,
        MainWindowViewModel initDataContext
        )
    {
        status = initFieldStatus;
        row = initRow;
        column = initColumn;
        DataMainWindowViewModel = initDataContext;
    }
    public fieldStatus status
    {
        get;
        set;
    }

    public bool isVisible
    {
        get
        {
            if (status == fieldStatus.FreeField) return false;
            return true;
        }
    }
    
    public string fieldColor
    {
        get
        {
            if (status == fieldStatus.WhiteField) return "LightSalmon";
            return "SaddleBrown";
        }
    }

    public void update()
    {
        RaisePropertyChanged(nameof(status));
        RaisePropertyChanged(nameof(row));
        RaisePropertyChanged(nameof(column));
        RaisePropertyChanged(nameof(fieldStatus));
        RaisePropertyChanged(nameof(isVisible));
        RaisePropertyChanged(nameof(fieldColor));
    }
    public void putChip()
    {
        status = DataMainWindowViewModel.activePlayer;
        DataMainWindowViewModel.swipePlayer();
        update();
        
        Console.WriteLine(row);
    }

    public bool enabled { get; set; } = false;

}

public enum fieldStatus
{
    FreeField, BlackField, WhiteField, 
}