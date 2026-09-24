# Modular AI System
**Status**: **`WIP`**

# Overview
A modular Behaviour-Tree based AI architecture capable of capturing programmable stimuli.

# Requirements
- [Unity's Behavior Library](https://docs.unity3d.com/Packages/com.unity.behavior@1.0/manual/behavior-features.html)

# Documentation

## Agent
An Agent is considered to be any sort of Entity that can "think" in the game (e.g. A Guard).
The base requirements for a GameObject to be considered an Agent is that it must contain an AIBrain Script and a Behaviour Tree component.
## Stimuli
Stimuli are changes in the Environment that can be detected by Agents. Agents can contain many Stimuli Receptors that allow them to react to events accordingly (e.g. Seeing something or Hearing something).
## Behaviour Tree (BT)
A BT is used to model the intent of an Agent given its current state. Things that can affect an Agents intent are:
- The Environment
    - Where is the Agent? Is he on dangerous grounds?
- The Mental and Physical State of the Agent
    - How is the Agent doing? Is he injured? Is he scared?
- Stimuli
    - Has the Agent seen or heard something?
- Objectives
    - Each Agent should have at least one main objective that they wish to accomplish. This objective can take a finite or infinite amount of time. (E.g. Rescuing all scientists, Roaming around the facility for prey, etc...)
The Behaviour Tree State will be driven by interruptions made by an Agents AIBrain when detecting certain Stimuli.
### Blackboard
A Blackboard will be used for communication between leaf nodes.
## Modules (MAY BE EXPANDED)
Modules model actions and reasonings that an Agent can perform.
These actions range from a Movement Controller that controls how an Agent moves, to a Tactical Positioning Module that allows an Agent to position themself in such a way to gain an advantage over an enemy.
To use modules, the "AIModulesDatabase" class must be used to retrieve a module. **ACCESS OF MODULES CAN ONLY BE DONE ON "Start()"!!!**
There are several types of modules, and they can be distinguished by certain keywords in their names (usually at the end).
### Brain
The brain is a special module that is responsible for processing stimuli.
### Stimuli Receptors
These modules are in charge of capturing certain stimuli and sending them to the brain. They usually end with a word that clearly relates to a stimuli (e.g. AI**Hearing**)
### Services
Services are constantly running in the background and can usually provide services to other components (e.g. AIThreatAssessorService constantly checks if the threat in focus is still visible or not, and can provide the last seen position of said threat to components requesting it.)
### Behaviours
Behaviours run continuously when **activated**.
A Behaviour modifies or controls some aspect of an Agent over time. For example, the "AITacticalPositioningBehaviour" makes it so that an Agent is constantly repositioning themselves to gain an advantage over a threat.
Multiple Behaviours can be activated at the same time, but some may conflict with others.
Behaviours do not usually provide any services to other components.
**Behaviours must always either be shut down by a Behaviour Tree Leaf, or have shutdown conditions themselves. If a Behaviour shouldn't be shut down, it is probably a Service**.
### Controllers
Controllers are used to allow an AI Agent to interact with other components (e.g. Their own Rig, a Firearm, etc...)
### Evaluators
Evaluators are used to retrieve/calculate information about a certain subject.
## Behaviour Trees and Modules relation
BTs and Modules work tightly together. BTs will **always** call Modules to be able to achieve a certain goal, depending on the Agent's intent. Modules model actions that an Agent wants to take depending on their intent.
Complex Logic should **never** be written in a BT leaf node, leaf nodes should only call Modules.

# Notes
**To do:**
- Test the framework of the system

This system is currently **`WIP`**. Some functionality may be incomplete or subject to change.