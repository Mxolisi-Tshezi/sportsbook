# Combination Technical Test

Welcome to Combination's technical test. **Please read this whole document carefully before beginning**.

In this test, you are provided with the following:

- A specification document `Combination Technical Test Specifications.pdf` detailing the expected behavior of a simple game in C#. The game follows the traditional rules of Tetris most people are familiar with, but we've included a specification of the whole game as a reference.
- A .NET solution containing the skeleton projects.

## Tasks

In this assignment, you will implement a simple version of the classic game of Tetris (see e.g. <https://www.youtube.com/watch?v=d7fajYz0r68> to get a feeling for what the game is about).
The purpose of the project is not to take the actual game of Tetris to the next level but rather to test your problem solving skills. The idea of a game is just there to define the boundaries and scope of the project.
When we measure project success the emphasis will be on:

- Problem-solving techniques and methodologies
- Software design

Since we don’t want you to get stuck on technicalities such as WPF and XNA Game Studio, we have provided you with a simple Windows Forms application written in C# which includes some of the tools you need to complete the task. Your mission will be to connect the dots and write the business logic for the entire game.

You will start from a skeleton of the application. Open "Tetris.sln" in Visual Studio, then compile and run the application. As you can see, the code for displaying the information on screen is already there, and you don't have to modify or extend it. Open the GameLogic class. This is most likely where you will make changes, among other things it contains key down/up handlers. More specifically, you should not have to modify any code in the UI directory. You will most likely add new classes to the project.

We suggest that you read through the `Combination Technical Test Specifications.pdf` document to get a feeling for the task ahead. When starting to code, try to solve the problems in the order they appear in the document, working in small steps to make sure that what you have implemented works well before proceeding to the next task.

Clearly, time is one of the constraints of this test but please remember that even if you don’t finish in time it doesn’t necessarily mean that you have failed.

On behalf of the Combination development team, we want to wish you good luck!

## Implementation

- The project must be implemented using the provided Tetris Forms Application.
- The provided classes can be extended, improved and changed as you see fit.

### Tetris Forms Application

To get you started we have provided you with a simple windows forms application which sets up the basic components in the Tetris game environment. In the Tetris solution you will find the following structure:

- Tetris.UI project
  - GameView.cs
  - GameViewConfig.cs
  - GameLogicBase.cs
- Tetris project
  - Program.cs
  - GameLogic.cs

#### Tetris.UI project

##### GameView.cs

The `GameView` is a representation of the window displayed in your application. It is responsible for drawing the game elements. It is also responsible for registering timer and keyboard event handlers.

Example interaction can be seen in the constructor of `GameLogic`. It demonstrates how to change the color of individual cells in the `GameView` main area and help area.

Note that cell information is managed internally in GameView. The `OnPaint` method is activated on an interval and draws the current state of the game.

##### GameViewConfig.cs

This class defines a few basic properties of the game. You will probably not need to change any
of the code contained within.

##### GameLogicBase.cs

This base-class contains basic functionality that connects the logic to the UI.

#### Tetris project

##### Program.cs

Contains the Main method where program execution begins and ends. Creates and connects the different entities in the system.

##### GameLogic.cs

The `GameLogic` class is quite lean as you open it, but will eventually need to contain or access logic that drives the game. Besides creating new files and classes, this is the only file you are expected to change. Start your implementation here.

To mention a couple of things, it will need to provide Tetris piece management, collision detection and handling, game scoring, etcetera.

## External tools

You are free to access any web resources but expected to be able to explain and reason about your choices and solutions.

## Evaluation

The test is meant to evaluate your programming and problem-solving skills. Runnable (and bug-free) code is always preferred over good reasoning where applicable.
