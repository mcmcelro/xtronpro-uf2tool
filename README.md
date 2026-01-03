# xtronpro-uf2tool

A simple command-line tool for converting NES ROM files into the format used by [the XTron Pro's NES emulator](https://www.ovobot.cc/en/weblog/2021/02/22/nes-emulator-xtron-pro/).

Based on the [Microsoft UF2 specification](https://github.com/microsoft/uf2) and a reverse-engineered understanding of the output of [the official tool from Ovobot](https://www.ovobot.cn/en/product/learn/uf2conv/).

## Requirements

* An XTron Pro handheld device
* An NES ROM 256KB or smaller
* .NET 10 SDK or later

The output file can be copied directly to the XTron Pro when it is connected to a computer in bootloader mode.

## Some Technical Details
* The converted file contains series of 512-byte blocks with specific headers and footers. The XTron Pro's NES emulator expects the NES ROM data to be embedded within these blocks in a specific way:
* The NES ROM data is divided into 256-byte chunks. As a result, the output file will be at least twice the size of the original NES ROM file.
* Per the UF2 specification, each block begins with two magic numbers - 0x0A324655, then 0x9E5D5157.
* The flags for each block are set to indicate that it is a "family ID" byte and that it contains data.
* The target address for each block is set to 0x08000000 plus the offset of the chunk within the NES ROM (that is, 0x08000000 plus 0x100 times the block number, counting from 0).
* The payload size is set to 256 bytes.
* The block number counts up from 0.
* The total number of blocks is calculated based on the size of the NES ROM, rounding up to the nearest whole block.
* The family ID is 0x57755A57, indicating [the STM32F4 MCU family](https://www.st.com/en/microcontrollers-microprocessors/stm32f4-series.html).
* Each block ends with a magic number of 0x0AB16F30.

## Licensing

Written by Michael McElroy and licensed under the MIT License. See the [LICENSE.txt](LICENSE.txt) file for details.