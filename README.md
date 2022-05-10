# filesizeanalysis
v0.2beta

TLDR - if you have very full drives, it takes a long time to startup (patience), there are potential stability issues.

This is a project I started to track down disk usage and where all the space is being used up.  I also added the functionality to watch for file changes to see which files are being touched on the system. This is still early software but becoming more functional. That said, this is very handy to seeing where all the disk consumption is occuring and finding / managing / deleting large files.

11/22/21
favs will now scan all local drives (even USB drives if you have them connected).  The scan only occurs at startup, due to recursive storage scans potentially being resource intensive (meaning lots of CPU usage and memory consumption). In this update the program has better file size information, and will retrieve the correct file sizes on compressed NTFS volumes.

5/9/22
favs will now request to run in elevated mode and targets only windows 10. The file change output is now recorded to a tree view box for drilling down on specific folders and files to see all the specific events on a given file / folder (with timestamp).
