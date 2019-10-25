import sys

ref = sys.argv[1]

if not ref.startswith('refs/heads/'):
    print('')
else:
    print(ref.split('/')[-1])