# dotnet-vs-cli

A simple .NET Global tool to launch Visual Studio from the CLI on Windows.

## Installation

```shell
λ dotnet tool install -g dotnet-vs-cli
```

## Usage

To open a solution file in a specific folder:
```shell
λ vs-launch -s Your.Solution.sln -p "C:\dev\your-project"
```
To open a solution file in this folder:
```shell
λ vs-launch -s Your.Solution.sln
```
To open any solution file in this folder (when you know there's only one):
```shell
λ vs-launch
```
To open any solution file in this folder with Admin right:
```shell
λ vs-launch --sudo
```

## Why?

First of all, this is an experimental project. I want to learn how to develop
a dotnet tool to help me do things faster.

I work with Visual Studio every day, and it bugs me when I cannot just open it
by a simple command line. To add insult to injury, some of our solutions need
to be opened with Administrator permission to be able to debug. Hence, this tool!