
from setuptools import setup, find_packages
from codecs import open
from os import path

here = path.abspath(path.dirname(__file__))

with open(path.join(here, 'README.rst')) as f:
	long_desc = f.read()

with open(path.join(here, 'VERSION')) as f:
	version_info = f.read().strip()

setup(
	name='python_utils',

	version=version_info,

	description='Tools and utilities for python microservices',
	long_description=long_desc,

	url='https://example.com',

	author='Tom English',
	author_email='redsoxfantom@gmail.com',

	classifiers=[
		'Development Status :: 1 - Planning',
		'Intended Audience :: Developers'
	],

	keywords='microservices tools utilities',

	packages=find_packages(),

	install_requires=['kazoo'],
)
