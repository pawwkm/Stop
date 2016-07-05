Script Example
==============

The scripting has a lot in common with **Diskpart**, however the scripting here goes beyond partition. It extends to file operations.

```
select disk "D:\Test.bin"
create mbr
create partition 1 offset 2048 sectors 65536
select partition 1
format fat32

move "D:\Script.txt" to "/"
```

Note that paths starting with / is considered a path on the virtual file system.
