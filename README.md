# ConsoleAuto
Library that automates console argument parsing and script automation.

![.NET Core](https://github.com/zeppaman/ConsoleAuto/workflows/.NET%20Core/badge.svg)
[![Nuget](https://img.shields.io/nuget/v/ConsoleAuto)](https://www.nuget.org/packages/ConsoleAuto/)
[![Nuget](https://img.shields.io/nuget/dt/ConsoleAuto)](https://www.nuget.org/packages/ConsoleAuto/)


## Why use this parsing library?
Command line parsing is something that usually is approached in this way: you define argument syntax, you parse it using command line parser library and then you use the parameter into the applciation.
This approach need many steps and it is hard to mantain. This library collapse all steps in just one: describe your method.

You just need to declare a method somewhere and add an annotation on it to let things goes in automatic. Another interesting scenario is the execution of multiple commands by config yaml file (similar to the docker files mechanism).

See the exampe to learn how this will simplify your console application.

```
Install-Package ConsoleAuto
```


## Features

- automatic info definition
- interactive mode
- automatic parameter mapping
- use dependency injection 
- can run a sequence of command allowing command scripting

## How it work

### Simple ussage

```cs
class Program
{
    static void Main(string[] args)
    {
        ConsoleAuto.Config(args)
            .LoadCommands()
            .Run();
    }

    [ConsoleCommand]
    public void MyMethod(string inputOne, int inputTwo)
    {
        //do stuff here
	}
}
```

this program will be usable running:

```bash
Myprogram MyMethod --inputOne xxxx --inputTwo 23

```

Each method annotated by ConsoleCommand is available for being invoked. Method can be in classes ouside the program file, static or not. Non static classes are created using DI and can receive dependencies in the costructor.
The default command is the info one, that shows usage information (generated automatically from command definitions)

```cs
public class IOCommands
{
    public ConsoleService consoleService;
    public IOCommands(ConsoleService consoleService)
    {
        this.consoleService = consoleService;
    }

    [ConsoleCommand(isPublic: false,info:"provide  this description")]
    public void WriteText(string text, bool newline)
    {
        //do stuff here 
    }
}
```

### Advanced usage


```cs
  ConsoleAuto.Config(args, servicecollection)// pass servicecollection to use the same container of main application
    .LoadCommands() //Load all commands from entry assembly + base commands
    .LoadCommands(assembly) //Load from a custom command
    .LoadFromType(typeof(MyCommand)) //load a single command
    .Register<MyService>() // add a service di di container used in my commands
    .Register<IMyService2>(new Service2()) // add a service di di container used in my commands, with a custom implementation
    .Default("MyDefaultCommand") //specify the default action if app starts without any command
    .Configure(config => { 
        //hack the config here
    })
    .Run();
```

#### Configuration options
| Method  | Note |
| ------------- | ------------- |
| Config  | inf service collection is null, creates an internal container for DI  |
| LoadCommand  | Load all commands from entry assembly + base commands, basing on annotation  |
| Register  | register a dependency for commands. If you are using DI on main application the dependency will be added on main DI container also. If you are using DI and you already added dependency you do not need to register.  |
| Configure  | this exposes the resulting configuration. Use at your risk. |
| Run  | Execute the program  |
| Default  | the command executed if no command is specified. default value is the info command. |


#### Command options

| Parameter  | Note |
| ------------- | ------------- |
| Name  | the name of the command (will be used for launching it from console)  |
| IsPublic  |  if true it is visible on console info |
| Mode  | Can be on-demand,   |
| Order  | the execution order for non on-demand, BeforeCommand, or AfterCommand. The on deamand options is the default one and probably what you usually whant. The commands do nothing unless the user run it from command line. After and Before mode run the command *anytime* you run an ondemand command. For example, the "welcome" or "pres a key to close" are before and after event.    |


### Programmatic usage usage

This tool can be used to execute a set of action. In this mode you can define many atomic commands and then define the sequence using an external file. 

The settings file MyScript.yaml

```yaml
Commands:
    welcome_step1:
       Action: welcome
       Desctiption: This is the line of text that will shown first 
       Args:
            header: my text (first line)
    welcome_step2:
       Action: welcome
       Desctiption:  In this example we do it twice, to prove we can execute commands multiple times with different args.
       Args:
           header: my text (second line)
    main_step:
       Action: CommandOne
       Desctiption: This is a custom command that diplay the test. Yes, another dummy thing.
       Args:
         text: I'm the central command output!
State:
   text: myglobal



```

```bash
MyExec.exe  exec MyScript.yaml
```



[Icons made by Eucalyp from Flaticon](https://www.flaticon.com/authors/eucalyp)


## TODOS

- [ ] interactive mode
- [ ] overload for removing default commands/Disabling


