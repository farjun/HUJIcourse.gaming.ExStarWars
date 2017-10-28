using System;
using System.Collections.Generic;

namespace Infra.Collections {
public class ObjectPool<T> : IDisposable where T : class, new() {
    private class PooledItem {
        public T item;
        public bool isUsed;

        public PooledItem(T item) {
            this.item = item;
            this.isUsed = false;
        }
    }

    // Set to 0 to not allow auto expanding the pool.
    public int increaseBy = 0;

    private LinkedListNode<PooledItem> lastBorrowed;
    private Dictionary<T, LinkedListNode<PooledItem>> mapping;
    // The pool pairs each item with a flag that say if the item is in use (or "active").
    private LinkedList<PooledItem> pool;

    public ObjectPool(int poolSize = 10, int increaseBy = 0) {
        this.increaseBy = increaseBy;
        mapping = new Dictionary<T, LinkedListNode<PooledItem>>(poolSize);
        pool = new LinkedList<PooledItem>();

        for (int i = 0; i < poolSize; i++) {
            Add(new T());
        }
    }

    public void Dispose() {
        if (pool != null) {
            pool.Clear();
            pool = null;
        }
        if (mapping != null) {
            mapping.Clear();
            mapping = null;
        }
    }
    
    public void Add(T item) {
        var pair = new PooledItem(item); 
        var node = new LinkedListNode<PooledItem>(pair);
        pool.AddLast(node);
        mapping[item] = node;
     }

    public void ReturnAll() {
        if (pool == null) return;

        foreach (var entry in pool) {
            entry.isUsed = false;
        }
    }

    public bool HasActiveObjects() {
        foreach (var entry in pool) {
            if (entry.isUsed) return true;
        }
        return false;
    }

    public void ReturnIfContains(T item) {
        if (mapping.ContainsKey(item)) {
            Return(item);
        }
    }
    public void Return(T item) {
        DebugUtils.Assert(mapping.ContainsKey(item), "Attempt to return non existing value");
        DebugUtils.Assert(mapping[item].Value.isUsed, "Attempt to return unused");
        var node = mapping[item].Value;
        node.isUsed = false;
    }

    public T Borrow() {
        if (lastBorrowed == null) {
            lastBorrowed = pool.First;
        }
        var node = lastBorrowed;
        while (node != null && node.Value.isUsed) {
            node = node.Next;
        }
        if (node == null && lastBorrowed != pool.First) {
            // Start searching from the start to the last borrowed.
            node = pool.First;
            while (node != lastBorrowed && node.Value.isUsed) {
                node = node.Next;
            }
            if (node == lastBorrowed) {
                node = null;
            }
        }

        if (node != null) {
            DebugUtils.Assert(!node.Value.isUsed, "Assumed the node is not used");
            lastBorrowed = node;
            lastBorrowed.Value.isUsed = true;
            return lastBorrowed.Value.item;
        }

        // All items are active.
        if (increaseBy > 0) {
            lastBorrowed = pool.Last;
            for (int i = 0; i < increaseBy; i++) {
                Add(new T());
            }
            lastBorrowed = lastBorrowed.Next;
            lastBorrowed.Value.isUsed = true;
            return lastBorrowed.Value.item;
        }
        return null;
    }
}
}
