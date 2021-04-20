using System.Collections.Generic;

namespace ImoutoDesktop.Scripting
{
    enum SelectType
    {
        Random,
        Nonoverlap,
        Sequential,
        Array,
        Void,
    }

    class Selector
    {
        public Selector()
        {
            num = new List<int>();
            roundorder = new List<int>();
            values = new List<List<int>>();
            values.Add(new List<int>());
        }

        public void AddBlock()
        {
            values.Add(new List<int>());
            ++blocknum;
        }

        public void Append(int line)
        {
            values[blocknum].Add(line);
        }

        public int[] Output(ExecutionContext vm)
        {
            if (blocknum == 0 && values[0].Count == 0)
            {
                return null;
            }
            switch (selectType)
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
            var result = new int[blocknum + 1];
            for (var i = 0; i <= blocknum; i++)
            {
                if (values[i].Count == 0)
                {
                    continue;
                }
                var index = vm.Random.Next(values[i].Count);
                result[i] = values[i][index];
            }
            return result;
        }

        private int[] ChoiceFromRoundOrder(ExecutionContext vm)
        {
            if (UpdateNums())
            {
                UpdateRoundOrder(vm);
            }
            var index = roundorder[c_index];
            var result = new int[blocknum + 1];
            if (blocknum != 0)
            {
                for (var i = 0; i <= blocknum; i++)
                {
                    var next = index / num[i];
                    result[i] = values[i][index - next * (num[i])];
                    index = next;
                }
            }
            else
            {
                result[0] = values[0][index];
            }
            lastroundorder = roundorder[c_index];
            ++c_index;
            if (c_index >= roundorder.Count)
            {
                UpdateRoundOrder(vm);
            }
            return result;
        }

        private int[] ChoiceArray()
        {
            var result = new List<int>();
            var length = values.Count;
            for (var i = 0; i < length; i++)
            {
                var count = values[i].Count;
                for (var j = 0; j < count; j++)
                {
                    result.Add(values[i][j]);
                }
            }
            return result.ToArray();
        }

        private bool UpdateNums()
        {
            var before_num = num.ToArray();
            var before_length = before_num.Length - 1;
            num.Clear();
            total = 1;
            var isChanged = blocknum != before_length;
            for (var i = 0; i <= blocknum; i++)
            {
                var count = values[i].Count;
                num.Add(count);
                total = total * count;
                if (i <= before_length)
                {
                    if (before_num[i] != count)
                    {
                        isChanged = true;
                    }
                }
            }
            return isChanged;
        }

        private void UpdateRoundOrder(ExecutionContext vm)
        {
            c_index = 0;
            roundorder.Clear();
            roundorder.Capacity = total;
            for (var i = 0; i < total; i++)
            {
                if (i != lastroundorder)
                {
                    roundorder.Add(i);
                }
            }
            if (roundorder.Count == 0)
            {
                roundorder.Add(0);
            }
            if (selectType == SelectType.Nonoverlap)
            {
                RandomShuffle<int>(ref roundorder, vm);
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

        private int blocknum;
        private int total;
        private int c_index;
        private int lastroundorder;
        private List<int> num;
        private List<int> roundorder;
        private List<List<int>> values;

        private SelectType selectType;

        internal SelectType SelectType
        {
            get { return selectType; }
            set { selectType = value; }
        }
    }
}
