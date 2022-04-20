using System.Collections.Generic;

namespace ImoutoDesktop.Scripting;

internal enum SelectType
{
    Random,
    Nonoverlap,
    Sequential,
    Array,
    Void
}

internal class Selector
{
    public void AddBlock()
    {
        _values.Add(new List<int>());
        ++_blocknum;
    }

    public void Append(int line)
    {
        _values[_blocknum].Add(line);
    }

    public int[] Output(ExecutionContext vm)
    {
        if (_blocknum == 0 && _values[0].Count == 0)
        {
            return null;
        }
        switch (SelectType)
        {
            case SelectType.Nonoverlap:
            case SelectType.Sequential:
                return ChoiceFromRoundOrder(vm);
            case SelectType.Array:
                return ChoiceArray();
            case SelectType.Void:
                return null;
            case SelectType.Random:
            default:
                return ChoiceRandom(vm);
        }
    }

    private int[] ChoiceRandom(ExecutionContext vm)
    {
        var result = new int[_blocknum + 1];
        for (var i = 0; i <= _blocknum; i++)
        {
            if (_values[i].Count == 0)
            {
                continue;
            }
            var index = vm.Random.Next(_values[i].Count);
            result[i] = _values[i][index];
        }
        return result;
    }

    private int[] ChoiceFromRoundOrder(ExecutionContext vm)
    {
        if (UpdateNums())
        {
            UpdateRoundOrder(vm);
        }
        var index = _roundorder[_cIndex];
        var result = new int[_blocknum + 1];
        if (_blocknum != 0)
        {
            for (var i = 0; i <= _blocknum; i++)
            {
                var next = index / _num[i];
                result[i] = _values[i][index - next * (_num[i])];
                index = next;
            }
        }
        else
        {
            result[0] = _values[0][index];
        }
        _lastroundorder = _roundorder[_cIndex];
        ++_cIndex;
        if (_cIndex >= _roundorder.Count)
        {
            UpdateRoundOrder(vm);
        }
        return result;
    }

    private int[] ChoiceArray()
    {
        var result = new List<int>();
        var length = _values.Count;
        for (var i = 0; i < length; i++)
        {
            var count = _values[i].Count;
            for (var j = 0; j < count; j++)
            {
                result.Add(_values[i][j]);
            }
        }
        return result.ToArray();
    }

    private bool UpdateNums()
    {
        var beforeNum = _num.ToArray();
        var beforeLength = beforeNum.Length - 1;
        _num.Clear();
        _total = 1;
        var isChanged = _blocknum != beforeLength;
        for (var i = 0; i <= _blocknum; i++)
        {
            var count = _values[i].Count;
            _num.Add(count);
            _total = _total * count;
            if (i <= beforeLength)
            {
                if (beforeNum[i] != count)
                {
                    isChanged = true;
                }
            }
        }
        return isChanged;
    }

    private void UpdateRoundOrder(ExecutionContext vm)
    {
        _cIndex = 0;
        _roundorder.Clear();
        _roundorder.Capacity = _total;
        for (var i = 0; i < _total; i++)
        {
            if (i != _lastroundorder)
            {
                _roundorder.Add(i);
            }
        }
        if (_roundorder.Count == 0)
        {
            _roundorder.Add(0);
        }
        if (SelectType == SelectType.Nonoverlap)
        {
            RandomShuffle(ref _roundorder, vm);
        }
    }

    /// <summary>
    /// 配列に対してランダムシャッフルを行います。
    /// </summary>
    /// <param name="shuffle">ランダムシャッフルする対象の配列。</param>
    private static void RandomShuffle<T>(ref List<T> shuffle, ExecutionContext vm)
    {
        if (shuffle.Count < 2)
        {
            return;
        }
        var length = shuffle.Count - 1;
        for (var i = 1; i < length; ++i)
        {
            var temp = shuffle[i];
            var index = vm.Random.Next(i + 1);
            shuffle[i] = shuffle[index];
            shuffle[index] = temp;
        }
    }

    private int _blocknum;
    private int _total;
    private int _cIndex;
    private int _lastroundorder;
    private readonly List<int> _num = new();
    private List<int> _roundorder = new();
    private readonly List<List<int>> _values = new() { new() };

    internal SelectType SelectType { get; set; }
}
