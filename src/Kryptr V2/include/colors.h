#pragma once
#ifndef _COLORS_
#define _COLORS_

#if defined(WIN32) || defined(_WIN32) || defined(__WIN32) && !defined(__CYGWIN__)
#include <Windows.h>

#define BLACK 0
#define BLUE 1
#define GREEN 2
#define CYAN 3
#define RED 4
#define MAGENTA 5
#define BROWN 6
#define LIGHTGREY 7
#define DARKGREY 8
#define LIGHTBLUE 9
#define LIGHTGREEN 10
#define LIGHTCYAN 11
#define LIGHTRED 12
#define LIGHTMAGENTA 13
#define YELLOW 14
#define WHITE 15
#define BLINK 128

class Colors {
public:
    static inline void TextColor(int fontcolor, int backgroundcolor)
    {
        static HANDLE screen; //Hacky way to prevent GetStdHandle running multiple times
        if (screen == nullptr) {
            screen = GetStdHandle(STD_OUTPUT_HANDLE);
        }
        int color_attribute;
        color_attribute = backgroundcolor;
        color_attribute = _rotl(color_attribute, 4) | fontcolor;
        SetConsoleTextAttribute(screen, color_attribute);
    }
};
#else
#include <stdlib.h>
#define RST  "\x1B[0m"
#define RED  "\x1B[31m"
#define GREEN  "\x1B[32m"
#define YELLOW "\x1B[33m"
#define BLUE  "\x1B[34m"
#define MAGENTA  "\x1B[35m"
#define CYAN  "\x1B[36m"
#define WHITE  "\x1B[37m"
#define BLACK "null" //placeholder

class Colors {
public:
    static inline constexpr void TextColor(const char* COLOR, const char* _) {
        std::cout << COLOR;
    }
};
#endif 
#endif /* _COLORS_ */