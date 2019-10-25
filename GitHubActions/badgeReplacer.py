import sys

badgeName = sys.argv[1]
repository = sys.argv[2]
branch = sys.argv[3]

changedFile = False

with open('readme.md') as f:
    readme = ''

    for lineNumber, line in enumerate(f):
        if badgeName == 'build' and lineNumber == 0:
            newLine = "[![Build status](https://github.com/{}/workflows/Build/badge.svg?branch={})](https://github.com/{}/workflows/Build/badge.svg?branch={})\n".format(
                repository,
                branch,
                repository,
                branch
            )
        elif badgeName == 'test' and lineNumber == 1:
            newLine = "[![Test status](https://github.com/{}/workflows/Test/badge.svg?branch={})](https://github.com/{}/workflows/Test/badge.svg?branch={})\n".format(
                repository,
                branch,
                repository,
                branch
            )
        else:
            newLine = line

        if newLine != line:
            changedFile = True
        
        readme += newLine

if changedFile:
    with open('readme.md', 'w') as f:
        f.write(readme)

print("::set-env changedReadme={}".format((changedFile and "1") or "0"))