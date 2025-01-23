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
    public field[,] fieldsArray { get; set; }
    
    public bool botPlayer { get; set; } = false;

    public string activePlayerColor
    {
        get
        {
            if (activePlayer == fieldStatus.WhiteField) return "LightSalmon";
            return "SaddleBrown";
        }
    } 

    public MainWindowViewModel()
    {
        initData();
    }
    

    public void initData()
    {
        gameEnd = false;
        botPlayer = false;
        activePlayer = fieldStatus.WhiteField;
        RaisePropertyChanged(nameof(activePlayerColor));
        RaisePropertyChanged(nameof(gameEnd));
        RaisePropertyChanged(nameof(botPlayer));
        fields = new ObservableCollection<field>();
        fieldsArray = new field[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if ((i == 3 && j == 3)||(i == 4 && j == 4))
                {
                    field f = new field(
                        initFieldStatus: fieldStatus.WhiteField,
                        initColumn: j,
                        initRow: i,
                        initDataContext: this);
                    fields.Add(f);
                    fieldsArray[i, j] = f;
                    fields[fields.Count-1].update();
                    continue;
                }
                if ((i == 3 && j == 4)||(i == 4 && j == 3))
                {
                    field f = new field(
                        initFieldStatus: fieldStatus.BlackField,
                        initColumn: j,
                        initRow: i,
                        initDataContext: this);
                    fields.Add(f);
                    fieldsArray[i, j] = f;
                    fields[fields.Count-1].update();
                    continue;
                }
                field f2 = new field(
                    initFieldStatus: fieldStatus.FreeField,
                    initColumn: j,
                    initRow: i,
                    initDataContext: this);
                fields.Add(f2);
                fieldsArray[i, j] = f2;
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
    
    public fieldStatus waitingPlayer
    {
        get
        {
            if (activePlayer == fieldStatus.BlackField)
            {
                return fieldStatus.WhiteField;
            }
            return fieldStatus.BlackField;
        }
    }

    public void swipePlayer()
    {
        if (activePlayer == fieldStatus.BlackField) activePlayer = fieldStatus.WhiteField;
        else activePlayer = fieldStatus.BlackField;
        enabledMove();
        RaisePropertyChanged(nameof(activePlayerColor));
        RaisePropertyChanged(nameof(gameEnd));
        RaisePropertyChanged(nameof(gameEndWhitePoints));
        RaisePropertyChanged(nameof(gameEndBlackPoints));
        foreach (var item in fields)
        {
            if (item.status != fieldStatus.FreeField) continue;
            if (item.enabled == true)
            {
                return;
            }
        }
        if (movePassed)
        {
            gameEnd = true;
            RaisePropertyChanged(nameof(gameEnd));
            RaisePropertyChanged(nameof(gameEndWhitePoints));
            RaisePropertyChanged(nameof(gameEndBlackPoints));
            return;
        }
        
        movePassed = true;
        swipePlayer();
    }
    
    
    
    private bool movePassed = false;
    public bool gameEnd { get; set; } = false;

    public int gameEndWhitePoints 
    {
        get
        {
            int points = 0;
            foreach (var item in fields)
            {
                if (item.status == fieldStatus.WhiteField) points++;
            }
            return points;
        }
    }
    public int gameEndBlackPoints 
    {
        get
        {
            int points = 0;
            foreach (var item in fields)
            {
                if (item.status == fieldStatus.BlackField) points++;
            }
            return points;
        }
    }
    public void enabledMove()
    {
        foreach (var item in fields)
        {
            item.left = leftTest(item);
            item.leftTop = leftTopTest(item);
            item.top = topTest(item);
            item.rightTop = rightTopTest(item);
            item.right = rightTest(item);
            item.rightBottom = rightBottomTest(item);
            item.bottom = bottomTest(item);
            item.leftBottom = leftBottomTest(item);
            item.update();
        }
        
    }
    
    public int leftTest(field f)
    {
        int i=1;
        int count = 0;
        while (f.column-i>=0)
        {
            if (fieldsArray[f.row,f.column-i].status == activePlayer)
            {
                return count;
            }
            if (fieldsArray[f.row,f.column-i].status == fieldStatus.FreeField)
            {
                return 0;
            }
            i++;
            count++;
        }
        return 0;
    }
    public int leftTopTest(field f)
    {
        int i=1, j=1;
        int count = 0;
        while (f.row-i>=0 && f.column-j>=0)
        {
            if (fieldsArray[f.row-i,f.column-j].status == activePlayer)
            {
                return count;
            }
            if (fieldsArray[f.row-i,f.column-j].status == fieldStatus.FreeField)
            {
                return 0;
            }
            i++;
            j++;
            count++;
        }
        return 0;
    }
    public int topTest(field f)
    {
        int i=1;
        int count = 0;
        while (f.row-i>=0)
        {
            if (fieldsArray[f.row-i,f.column].status == activePlayer)
            {
                return count;
            }
            if (fieldsArray[f.row-i,f.column].status == fieldStatus.FreeField)
            {
                return 0;
            }
            i++;
            count++;
        }
        return 0;
    }
    public int rightTopTest(field f)
    {
        int i=1, j=1;
        int count = 0;
        while (f.row-i>=0 && f.column+j<8)
        {
            if (fieldsArray[f.row-i,f.column+j].status == activePlayer)
            {
                return count;
            }
            if (fieldsArray[f.row-i,f.column+j].status == fieldStatus.FreeField)
            {
                return 0;
            }
            i++;
            j++;
            count++;
        }
        return 0;
    }
    public int rightTest(field f)
    {
        int i=1;
        int count = 0;
        while (f.column+i<8)
        {
            if (fieldsArray[f.row,f.column+i].status == activePlayer)
            {
                return count;
            }
            if (fieldsArray[f.row,f.column+i].status == fieldStatus.FreeField)
            {
                return 0;
            }
            i++;
            count++;
        }
        return 0;
    }
    public int rightBottomTest(field f)
    {
        int i=1, j=1;
        int count = 0;
        while (f.row+i<8 && f.column+j<8)
        {
            if (fieldsArray[f.row+i,f.column+j].status == activePlayer)
            {
                return count;
            }
            if (fieldsArray[f.row+i,f.column+j].status == fieldStatus.FreeField)
            {
                return 0;
            }
            i++;
            j++;
            count++;
        }
        return 0;
    }
    public int bottomTest(field f)
    {
        int i=1;
        int count = 0;
        while (f.row+i<8)
        {
            if (fieldsArray[f.row+i,f.column].status == activePlayer)
            {
                return count;
            }
            if (fieldsArray[f.row+i,f.column].status == fieldStatus.FreeField)
            {
                return 0;
            }
            i++;
            count++;
        }
        return 0;
    }
    public int leftBottomTest(field f)
    {
        int i=1, j=1;
        int count = 0;
        while (f.row+i<8 && f.column-j>0)
        {
            if (fieldsArray[f.row+i,f.column-j].status == activePlayer)
            {
                return count;
            }
            if (fieldsArray[f.row+i,f.column-j].status == fieldStatus.FreeField)
            {
                return 0;
            }
            i++;
            j++;
            count++;
        }
        return 0;
    }

    public void BotPlay()
    {
        while (!gameEnd)
        {
            
        
        foreach (var item in fields)
        {
            if (item.enabled == true && item.status == fieldStatus.FreeField)
            {
                var rand  = new Random();
                int numerRand = rand.Next(5593);
                if (numerRand % 2 == 1)
                {
                    item.putChip();
                    return;
                }
                
            }
            
        }
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
    if (
        left>0||
        leftTop > 0 ||
        top > 0 ||
        rightTop > 0 ||
        right > 0 ||
        rightBottom > 0 ||
        bottom > 0 ||
        leftBottom > 0
    )
        enabled = true;
    else
    {
        enabled = false;
    }
        RaisePropertyChanged(nameof(status));
        RaisePropertyChanged(nameof(row));
        RaisePropertyChanged(nameof(column));
        RaisePropertyChanged(nameof(fieldStatus));
        RaisePropertyChanged(nameof(isVisible));
        RaisePropertyChanged(nameof(fieldColor));
        RaisePropertyChanged(nameof(enabled));
    }
    public void putChip()
    {
        enabled = false;
        status = DataMainWindowViewModel.activePlayer;
        turnOverAll();
        // public int leftTop { get; set; }
        // public int top { get; set; }
        // public int rightTop { get; set; }
        // public int right { get; set; }
        // public int rightBottom { get; set; }
        // public int bottom { get; set; }
        // public int leftBottom { get; set; }
        
        DataMainWindowViewModel.swipePlayer();
        Console.WriteLine(DataMainWindowViewModel.activePlayer);
        if (DataMainWindowViewModel.botPlayer)
        {
            if (DataMainWindowViewModel.activePlayer == fieldStatus.BlackField) 
                DataMainWindowViewModel.BotPlay();
        }
        
        
        update();
    }

    public bool enabled { get; set; } = false;
    
    public int left { get; set; }
    public int leftTop { get; set; }
    public int top { get; set; }
    public int rightTop { get; set; }
    public int right { get; set; }
    public int rightBottom { get; set; }
    public int bottom { get; set; }
    public int leftBottom { get; set; }
    
    ////////////////////////////////////////////////////////
    public void leftTurnOver()
    {
        for (int i = column-1; i > 0 && left > 0; i--)
        {
            left--;
            DataMainWindowViewModel.fieldsArray[row, i].status = DataMainWindowViewModel.activePlayer;
        }
    }
    public void leftTopTurnOver()
    {
        int i=row-1, j=column-1;
        while (i>=0 && j>=0 && leftTop > 0)
        {
            leftTop--;
            DataMainWindowViewModel.fieldsArray[i, j].status = DataMainWindowViewModel.activePlayer;
            i--;
            j--;
        }
    }
    public void topTurnOver()
    {
        for (int i = row-1; i > 0 && top > 0; i--)
        {
            top--;
            DataMainWindowViewModel.fieldsArray[i, column].status = DataMainWindowViewModel.activePlayer;
        }
    }
    public void rightTopTurnOver()
    {
        int i=row-1, j=column+1;
        while (i>=0 && j<8 && rightTop > 0)
        {
            rightTop--;
            DataMainWindowViewModel.fieldsArray[i, j].status = DataMainWindowViewModel.activePlayer;
            i--;
            j++;
        }
    }
    public void rightTurnOver()
    {
        for (int i = column+1; i < 8 && right > 0; i++)
        {
            right--;
            DataMainWindowViewModel.fieldsArray[row, i].status = DataMainWindowViewModel.activePlayer;
        }
    }
    public void rightBottomTurnOver()
    {
        int i=row+1, j=column+1;
        while (i<8 && j<8 && rightBottom >0)
        {
            rightBottom--;
            DataMainWindowViewModel.fieldsArray[i, j].status = DataMainWindowViewModel.activePlayer;
            i++;
            j++;
        }
    }
    public void bottomTurnOver()
    {
        for (int i = row+1; i < 8 && bottom > 0; i++)
        {
            bottom--;
            DataMainWindowViewModel.fieldsArray[i, column].status = DataMainWindowViewModel.activePlayer;
        }
    }
    public void leftBottomTurnOver()
    {
        int i=row+1, j=column-1;
        while (i<8 && j>=0 && leftBottom>0)
        {
            leftBottom--;
            DataMainWindowViewModel.fieldsArray[i, j].status = DataMainWindowViewModel.activePlayer;
            i++;
            j--;
        }
    }

    public void turnOverAll()
    {
        leftTurnOver();
        leftTopTurnOver();
        topTurnOver();
        rightTopTurnOver();
        rightTurnOver();
        rightBottomTurnOver();
        bottomTurnOver();
        leftBottomTurnOver();
        
    }

}

public enum fieldStatus
{
    FreeField, BlackField, WhiteField, 
}