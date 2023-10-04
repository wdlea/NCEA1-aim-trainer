package linkedlist

// A linked list node is a single value in a linked list
type LinkedListNode[contained any] struct {
	Value contained

	Previous *LinkedListNode[contained]
	Next     *LinkedListNode[contained]
}

// adds a node with value as its value after the node, returns this node
func (n *LinkedListNode[contained]) Append(value contained, list *LinkedList[contained]) (appended *LinkedListNode[contained]) {
	appended = new(LinkedListNode[contained])

	appended.Next = n.Next
	appended.Previous = n
	appended.Value = value

	if appended.Next == nil {
		list.Last = appended

		if appended.Previous == nil {
			list.First = appended
		}
	} else {
		n.Next.Previous = appended
	}

	n.Next = appended

	return
}

// adds a node with value as its value before the node, returns this node
func (n *LinkedListNode[contained]) Prepend(value contained, list *LinkedList[contained]) (prepended *LinkedListNode[contained]) {
	prepended = new(LinkedListNode[contained])

	prepended.Next = n
	prepended.Previous = n.Previous

	prepended.Value = value

	if prepended.Previous == nil {
		list.First = prepended
		if prepended.Next == nil {
			list.Last = prepended
		}
	} else {
		n.Previous.Next = prepended
	}

	n.Previous = prepended

	return
}

//removes the node and joins the values before and after it, returns the value of the deleted node
func (n *LinkedListNode[contained]) Pop(list *LinkedList[contained]) (value contained) {
	if n.Next != nil {
		n.Next.Previous = n.Previous
	} else {
		list.Last = n.Previous
	}

	if n.Previous != nil {
		n.Previous.Next = n.Next
	} else {
		list.First = n.Next
	}

	value = n.Value

	return
}
