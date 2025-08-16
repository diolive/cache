namespace DioLive.Cache.Common;

public class Hierarchy<TEntity, TId> : IReadOnlyCollection<Hierarchy<TEntity, TId>.Node>
    where TId : struct
{
    private readonly Dictionary<TId, Node> _nodes;

    public Hierarchy(
        IEnumerable<TEntity> items,
        Func<TEntity, TId> idSelector,
        Func<TEntity, TId?> parentIdSelector
    )
    {
        List<TEntity> itemsCollection = [.. items.OrderBy(parentIdSelector).ThenBy(idSelector)];

        _nodes = itemsCollection.ToDictionary(
            item => idSelector(item),
            item => new Node(item)
        );

        foreach (TEntity item in itemsCollection)
        {
            if (parentIdSelector(item) is not TId parentId)
            {
                continue;
            }

            Node node = _nodes[idSelector(item)];
            Node parentNode = _nodes[parentId];

            node.Parent = parentNode;
            parentNode.Children.Add(node);
        }

        Roots = [.. _nodes.Values.Where(item => item.Parent == null)];

        foreach (Node root in Roots)
        {
            SetLevels(root, 0);
        }

        static void SetLevels(Node node, int level)
        {
            node.Level = level;
            foreach (Node childNode in node.Children)
            {
                SetLevels(childNode, level + 1);
            }
        }
    }

    public Node this[TId id] => _nodes[id];
    public IReadOnlyCollection<Node> Roots { get; }
    public int Count => _nodes.Count;

    public IEnumerator<Node> GetEnumerator()
    {
        return Roots.SelectMany(root => root.GetSubTree()).GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public class Node(TEntity value)
    {
        public TEntity Value { get; } = value;
        public Node? Parent { get; set; }
        public ICollection<Node> Children { get; } = [];
        public int Level { get; set; }

        public Node Root => Parent?.Root ?? this;

        public IEnumerable<TEntity> Values()
        {
            yield return Value;

            foreach (TEntity value in Children.SelectMany(c => c.Values()))
            {
                yield return value;
            }
        }

        public IEnumerable<Node> GetSubTree()
        {
            yield return this;

            foreach (Node child in Children.SelectMany(c => c.GetSubTree()))
            {
                yield return child;
            }
        }
    }
}

public static class Hierarchy
{
    public static Hierarchy<TEntity, TId> Create<TEntity, TId>(
        IEnumerable<TEntity> items,
        Func<TEntity, TId> idSelector,
        Func<TEntity, TId?> parentIdSelector
    )
        where TId : struct
    {
        return new Hierarchy<TEntity, TId>(
            items,
            idSelector,
            parentIdSelector
        );
    }
}