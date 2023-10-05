package linkedlist

// a linked list is an implementation of LinkedList
type LinkedList[contained any] struct {
	First *LinkedListNode[contained]
	Last  *LinkedListNode[contained]
}

// adds a node at the start of the list
func (l *LinkedList[contained]) AddFirst(value contained) (node *LinkedListNode[contained]) {
	if l.First != nil {
		return l.First.Prepend(value, l)
	} else {
		node = new(LinkedListNode[contained])
		node.Value = value

		l.First = node
		l.Last = node

		return
	}
}

// adds a node at the end of the list
func (l *LinkedList[contained]) AddLast(value contained) (node *LinkedListNode[contained]) {
	if l.Last != nil {
		return l.Last.Append(value, l)
	} else {
		node = new(LinkedListNode[contained])
		node.Value = value

		l.First = node
		l.Last = node

		return
	}
}
