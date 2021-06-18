# <h1 align="center">fiat ⚡</h1>
#### Fiat is latin term that means bringing things into existance. That's pretty much what this guy does... with files.

## What is this?

Shortly, is a tool for scaffolding directories and files based on stubs for classes, interfaces, methods, and whatever you provide. It is written in C# using .Netcore 3.1.

If you have to create several boilerplate files for any new implementation, **fiat is a tool for you.**

Is there any odertools doing that a

## Why?

Today's architectures have a common approach of "many objects doing few things" which is better than "few objects doing many things". That's great, but any new implmentation comes with a lot of boilerplate code. 

Take Command and Query Responsibility Segregation (CQRS), for example, generally you have separate project libs for commands, queries, persistence, model and the application itself. Any new implementation requires new folders, classes, interfaces etc.

## What if...

You could just run something like

> `fiat new-command Order NewProductItem` 

## Get started

1. Download the latest [release](https://github.com/diegosiao/fiat/releases/tag/v0.0.1-beta) or compile from the code in repository. **Put it in your PATH**;
2. Define the stubs files in some folder in your project;
3. Run `fiat init` to create the template file *fiat.config.json* in the root of your project;
4. Define properly the *fiat.config.json* template file created. Refer to the simple documentation below;
5. Run your first `fiat profile-name` command to create the files you defined;
6. Get amazed 😲

### Stubs

Stubs are files with the boilerplate content to create new files. Refer to the [/stubs](https://github.com/diegosiao/fiat/tree/master/stubs) folder in this repository for some reference and examples. You can use variables in these files to be replaced during the scaffolding creation.

**IMPORTANT: Variables should be put between '$'. E.g.: $variablename$. Take a look at the stubs in example. **

### [fiat.config.json](https://github.com/diegosiao/fiat/blob/master/fiat.config.json)

That's where you define the instructions for directory, file and insertions of code scaffolding for your profiles.
```json
{
  "baseDir": "A fully qualified directory path to which all files are relative to",
  "encoding": "The default encoding is UTF-8. Refer to [.Net enconding names](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding?view=net-5.0#list-of-encodings) if you need to change that. ",
  "profiles": [
    {
      "name": "profile-name",
      "vars": [ "entity", "action", "variablex", "kindofselfexplanatory" ],
      "stubsDir": "The directory for your stubs. Relative to the base directory defined or a fully qualified path. E.g. stubs/commands",
      "scaffolds": [
        {
          "name": "validator",
          "file": "my-project-model/$entity$/Commands/Validators/$entity$$action$Validator.cs",
          "stub": "Validator.stub",
          "put": { 
            "new-key": "File path to the file with contents to be inserted. A placeholder **fiat:new-key** is expected in the target file/stub.",
            "antoher-key": "A placeholder **fiat:another-key** is expected in the target file/stub."
          }
        }
      ]
    }
  ]
}
```

A typical command has the following structure

`fiat [profile-name] [var1value] [var2value]`

That is it. I hope it helps. Enjoy it!
