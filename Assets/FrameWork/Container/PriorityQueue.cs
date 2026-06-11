using System;
using System.Collections;
using System.Collections.Generic;

namespace GFW.Container
{ 
    public class PriorityQueue<T>:IEnumerable<T> 
    {
        List<T> Elements;
        Func<T, T, bool> Compare;
        public int Count=>Elements.Count;

        public void Print()
        {
            foreach(var element in Elements)
            {
                if(element!=null)
                {
                    UnityEngine.Debug.Log(element);
                }
            }
        }


        public PriorityQueue(Func<T, T, bool> compare)
        {
            Elements=new List<T>();
            Compare=compare;
        }

        public void Enqueue(T element)
        {
            Elements.Add(element);
            ShfitUp(Count-1);
        }
        public T Dequeue() 
        {
            if (Count == 0) return default(T);

            T element = Elements[0];
            if (Count > 1)
            {
                Elements[0] = Elements[Count - 1];
                Elements.RemoveAt(Count - 1);
                ShfitDown(0);
            }
            else
            {
                Elements.Clear();
            }
            return element;
        }
        public void Clear()
        {
            Elements.Clear();
        }
        public bool TryPeek(out T outVal)
        {
            outVal = default(T);
            if (Count>0)
            {
                outVal = Elements[0];
                return true;
            }
            return false;
        }


        private void ShfitDown(int elementIndex)
        {
            if (!IsValidIndex(elementIndex)) return;

            int left=GetLeft(elementIndex);
            int right=GetRight(elementIndex);
            T element = Elements[elementIndex];
            T leftElement=GetLeftElement(elementIndex);
            T rightElement=GetRightElement(elementIndex);
            if(IsValidIndex(left)&&IsValidIndex(right))
            {
                bool l_com= Compare(element, leftElement);
                bool r_com= Compare(element, rightElement);
                if (l_com&&r_com) return;

                if (!l_com)
                {
                    if (Compare(leftElement, rightElement))
                    {
                        Elements[elementIndex]=leftElement;
                        Elements[left]=element;
                        ShfitDown(left);
                    }
                    else
                    {
                        Elements[elementIndex]=rightElement;
                        Elements[right]=element;
                        ShfitDown(right);
                    }
                }
                else if(!r_com)
                {
                    if (Compare(rightElement,leftElement))
                    {
                        Elements[elementIndex]=rightElement;
                        Elements[right]=element;
                        ShfitDown(right);
                    }
                    else
                    {
                        Elements[elementIndex]=leftElement;
                        Elements[right]=element;
                        ShfitDown(left);
                    }
                }
            }
            else if(IsValidIndex(left))
            {
                bool l_com = Compare(element, leftElement);
                if(!l_com)
                {
                    Elements[elementIndex]=leftElement;
                    Elements[left]=element;
                    ShfitDown(left);
                }
            }
            else if(IsValidIndex(right))
            {
                bool r_com = Compare(element, rightElement);
                if (!r_com)
                {
                    Elements[elementIndex]=rightElement;
                    Elements[left]=element;
                    ShfitDown(right);
                }
            }
        }
        private void ShfitUp(int elementIndex)
        {
            if (!IsValidIndex(elementIndex)) return;
            int parent=GetParent(elementIndex);
            T element = Elements[elementIndex];
            T parentElement = GetParentElement(elementIndex);

            if(IsValidIndex(parent))
            {
                bool com = Compare(parentElement, element);
                if(!com)
                {
                    Elements[parent]=element; 
                    Elements[elementIndex]=parentElement;
                    ShfitUp(parent);
                }
            }
        }

        private int GetParent(int elementIndex)
        {
            int res = -1;
            if(elementIndex%2!=0)
            {
                res = elementIndex/2;
            }
            else
            {
                res = elementIndex/2-1;
            }
            return res;
        }
        private int GetLeft(int elementIndex)
        {
            int res = -1;
            res=elementIndex*2+1;
            return res;
        }
        private int GetRight(int elementIndex)
        {
            int res = -1;
            res=elementIndex*2+2;
            return res;
        }
        private T GetParentElement(int elementIndex)
        {
            int index = GetParent(elementIndex);
            if (index<0||index>=Count) return default(T);
            return Elements[index];
        }
        private T GetLeftElement(int elementIndex)
        {
            int index = GetLeft(elementIndex);
            if (index<0||index>=Count) return default(T);
            return Elements[index];
        }
        private T GetRightElement(int elementIndex)
        {
            int index = GetRight(elementIndex);
            if (index<0||index>=Count) return default(T);
            return Elements[index];
        }

        public bool IsValidIndex(int elementIndex)
        {
            if(elementIndex<0)return false;
            if(elementIndex>=Count)return false;
            return true;
        }
  
        public IEnumerator<T> GetEnumerator()
        {
            return Elements.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); 
        }


    }
}
