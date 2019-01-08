using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DioLive.Cache.Storage.Entities
{
	public class Hierarchy<TEntity, TId> : IEnumerable<Hierarchy<TEntity, TId>.Node>
		where TId : struct
	{
		private readonly Dictionary<TId, Node> _nodes;

		public Hierarchy(IEnumerable<TEntity> items, Func<TEntity, TId> idSelector, Func<TEntity, TId?> parentIdSelector)
		{
			_nodes = new Dictionary<TId, Node>();

			List<TEntity> itemsCollection = items.OrderBy(parentIdSelector).ThenBy(idSelector).ToList();

			foreach (TEntity item in itemsCollection)
			{
				var node = new Node
				{
					Value = item,
					Children = new List<Node>()
				};

				TId? parentId = parentIdSelector(item);
				if (parentId.HasValue)
				{
					Node parentNode = _nodes[parentId.Value];
					parentNode.Children.Add(node);
					node.Parent = parentNode;
				}

				_nodes.Add(idSelector(item), node);
			}

			foreach (Node item in _nodes.Values)
			{
				item.Children = item.Children.ToList().AsReadOnly();
			}

			Roots = _nodes.Values.Where(item => item.Parent == null).ToList().AsReadOnly();

			foreach (Node root in Roots)
			{
				SetLevels(root, 0);
			}

			void SetLevels(Node node, int level)
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

		public IEnumerator<Node> GetEnumerator()
		{
			return Roots.SelectMany(root => root.GetSubTree()).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public class Node
		{
			public TEntity Value { get; internal set; }
			public Node Parent { get; internal set; }
			public ICollection<Node> Children { get; internal set; }
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
}