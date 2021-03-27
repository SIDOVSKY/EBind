namespace EBind.Collections
{
    internal class LinkedMap
    {
        private Node? _head;
        private Node? _last;

        public ref object? GetOrAddValueRef(object key)
        {
            var currentNode = _head;

            while (currentNode is not null)
            {
                if (currentNode.Key == key)
                    return ref currentNode.Value;

                currentNode = currentNode.Next;
            }

            var newNode = new Node(key);

            if (_last is null)
            {
                _head = _last = newNode;
            }
            else
            {
                _head!.Next = _last;
                _last.Next = newNode;
                _last = newNode;
            }

            return ref newNode.Value;
        }

        private class Node
        {
            public object Key;
            public object? Value;

            public Node? Next;

            public Node(object key)
            {
                Key = key;
            }
        }
    }
}
