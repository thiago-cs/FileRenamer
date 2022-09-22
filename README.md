# FileRenamer

FileRenamer is a WinUI 3 utility that makes bulk managing files and folders easy.

## Patterns used in this app:

### Composition

File operations can be groupped and treated the same way as a single one. This allows treating individual operations and compositions uniformly.

### Bridge

* The name of a file/folder may be modified in different way. For example, a new value could be inserted at the beggining of the name.

* Different types of values may be inserted (e.g.: a counter or a given text).

* In turn, counter values may be formatted.

Each of these abstractoins are decoupled from the implementation so that the two can vary independently, e.g. new implementations can be added.

_(to be continued)_
