Before you copy my FAT32 "Implementation"
=========================================

While booting, the Raspberry Pi is looking for a few files. To be able to do that it expects that the SD card is formatted as Fat32.
Last but not least, the Pi loads the operating system. I wanted to use my own file system(s) so I wrote a quick and dirty implementation
that would allow me to create a small FAT32 partition to put on files that the Pi uses during boot and nothing else. Then have a second
partion with my own file system. This way the Pi can happily boot and Fat32 is no longer is no longer a chain around my ankle.

TL;DR The implementation is not complete and probably won't be. The implementation is not tested thoroughly. Look at the code 
for hints only. 