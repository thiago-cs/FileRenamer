# FileRenamer

FileRenamer is a WinUI 3 utility that makes bulk managing files and folders easy.

## Patterns used in this app:

### Composition

File operations can be groupped and treated the same way as a single one. This allows treating individual operations and compositions uniformly.

### Bridge

* The name of a file/folder may be modified in different way. For example, a new value could be inserted at the beggining of the name.

* Different types of values may be inserted (e.g.: a counter or a given text).

Each of these abstractoins are decoupled from the implementation so that the two can vary independently, e.g. new implementations can be added.

### Strategy Pattern

* The value of a counter may be formatted. The value generation logic is separated from the value formatting logic, which is defined (chosen) only during run-time


_(to be continued)_
