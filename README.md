# little-dan-computer
An open-source assembly simulator based on LMC (Little Man Computer), aimed to teach students basic assembly programming in a user-friendly virtual computer model environment.

[Check it out here!](https://danqzq.itch.io/little-dan-computer)

![](https://github.com/danqzq/little-dan-computer/blob/main/ldc.gif)

## [The Instruction Set](https://ldc.danqzq.games/instruction-set/)

## Unity Version
Made with Unity 6000.0.22f1

## Core Components:
Memory: the computer's memory
Registers:
IR (Instruction Register): holds the instruction to be executed
MAR (Memory Address Register): holds the current address that is being used for a read/write operation
MBR (Memory Buffer Register): holds data that is going to be written into a memory address
PC (Program Counter): holds the index of the current executing instruction
Accumulator: holds a number, used for performing arithmetic operations
## Additional Components:
Graphics Screen: displays each memory chunk as a color, being able to read and process the data in different color formats (RGB565, RGB555, RGB444 and etc.)

---
## Memory Handling Mechanism:
There are some ways LDC differentiates from the basic LMC - each memory chunk (or mailbox) has a storage of 2 bytes. The instructions are stored in the following way, considering the decimal format: starting from the left-hand side, the first three digits are used for storing the operand, while the rest of the two digits are used to store the opcode. An operand has a range of [-999; 999] and the opcode has a range of [0-31]. This allows for there to be 32 unique instructions (and an additional instruction without an opcode), due to the nature of short (16-bit) integers.


