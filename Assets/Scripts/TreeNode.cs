using System;
using System.Collections.Generic;
using System.Linq;

public class TreeNode<T> {
    private readonly T _value;
    private readonly LinkedList<TreeNode<T>> _children = new LinkedList<TreeNode<T>>();

    public TreeNode(T value) {
        _value = value;
    }

    public TreeNode<T> Parent { get; private set; }

    public T Value { get { return _value; } }

    public LinkedList<TreeNode<T>> Children { get { return _children; } }

    public TreeNode<T> AddChild(T value) {
        var node = new TreeNode<T>(value) { Parent = this };
        _children.AddLast(node);
        return node;
    }
    
    public bool RemoveChild(TreeNode<T> node) {
        return _children.Remove(node);
    }

    public void RemoveAllChildren() {
        _children.Clear();
    }

    public void Traverse(Action<T> action) {
        action(Value);
        foreach (var child in _children)
            child.Traverse(action);
    }

    public IEnumerable<T> Flatten() {
        return new[] { Value }.Union(_children.SelectMany(x => x.Flatten()));
    }
}