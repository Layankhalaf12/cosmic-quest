# 🚀 Cosmic Quest: Multiverse Challenge

A space-themed, single-player game hub built with **.NET MAUI** and **C#**. Players land on an animated solar system, answer a planet quiz to unlock the games, and then choose between three mini-games.

## Background

Cosmic Quest started as a three-person term project for PROG 10065 at Sheridan College (Summer 2025), where I built the **main hub page** and the **XO (tic-tac-toe) game**.

This repository is my personal version. It contains only my original work from that project, plus two new games added afterwards (**Asteroid Dodge** and **Planet Memory Match**) to replace my teammates' games. None of my teammates' code is included here.

## Features

### 🪐 Main hub (from the team project, my part)
- Six clickable planets animated on two orbit rings around a central planet, positioned each frame with `Math.Cos` / `Math.Sin` inside an `AbsoluteLayout` and driven by an `IDispatcherTimer`
- Orbit speed slider
- Clicking a planet opens a quiz question, stored in a jagged array. A correct answer unlocks the game menu; a wrong answer sends the player to a random game
- Shell navigation between all game pages

### ❌⭕ XO game (from the team project, my part)
- Player enters a name and chooses X or O; the computer opens when the player picks O
- Computer opponent moves after a short delay, using a dispatcher timer
- Row, column and diagonal win detection, draw detection and running scores, kept in a separate `GameLogic` class

### ☄️ Asteroid Dodge (added after the course)
- Steer a rocket left and right to dodge falling asteroids; every asteroid that passes scores a point
- Asteroids spawn faster as the score rises, with circle-based collision detection
- Pause and resume, plus a best-score tracker
- Rules live in `BusinessLogic/AsteroidLogic.cs`; the page only draws the game state

### 🌍 Planet Memory Match (added after the course)
- Six pairs of space symbols shuffled onto a 4 × 3 board with a Fisher-Yates shuffle
- Flip two cards per turn; mismatches turn back over after a short pause
- Move counter and pair tracker
- Rules live in `BusinessLogic/MemoryLogic.cs`

## Architecture

| Layer | Files |
|---|---|
| Presentation | `MainPage`, `XoGame`, `AsteroidGame`, `MemoryGame` (`.xaml` + code-behind) |
| Business logic | `BusinessLogic/GameLogic.cs`, `AsteroidLogic.cs`, `MemoryLogic.cs` |
| Navigation | `AppShell.xaml`: tab bar with one route per page |

Game rules are kept out of the UI code, so each page only reads state from its logic class and redraws.

UML class and sequence diagrams for the XO game are included (`XoGame UML.png`, `XoGame SEQUENCE.png`).

## Running

1. Open `GameLastOne.sln` in Visual Studio 2022 with the **.NET MAUI** workload installed
2. Select **Windows Machine** (or an Android emulator) as the target
3. Press **F5**

## Tech
C# · .NET 8 · .NET MAUI · XAML · Visual Studio · Visual Paradigm (UML)
