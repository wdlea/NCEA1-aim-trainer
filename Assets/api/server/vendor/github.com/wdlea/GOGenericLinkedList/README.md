# GOGenericLinkedList

> builtin package ***container/list* provides this functionality**, this one is a generic implementation

## Why use this instead of *container/list*?
*container/list* is also an implementation of a linked list data type in go. However, *container/list* uses the *any* datatype to store values. This one uses generics to store the value. This means that you won't have to perform any casts fron *any* to the data type you want. If you want to store data as *any* you could either use *container/list*, or instantiate your linked list like so:

    list := new(LinkedList[any])

## Example code
> For more examples you could check out the [unit test](/linkedList_test.go).

