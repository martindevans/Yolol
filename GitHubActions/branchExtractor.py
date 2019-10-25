import sys

ref = sys.argv[1]

if not ref.startswith('refs/heads/'):
    branch = ''
else:
    branch = ref.split('/')[-1]

print("::set-env branch={}".format(branch))