/*-------------------------------------------------------------------------------------*\

##	Copyright © 2019 Karim Takieddine
##
##	Permission is hereby granted, free of charge, to any person obtaining a copy
##	of this software and associated documentation files (the "Software"), to deal
##	in the Software without restriction, including without limitation the rights
##	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
##	of the Software, and to permit persons to whom the Software is furnished to do so,
##	subject to the following conditions:
##
##	The above copyright notice and this permission notice shall be included in all
##	copies or substantial portions of the Software.
##
##	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
##	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
##	PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
##	HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
##	CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
##	OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

\*-------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using UnityEngine;

public readonly struct Triangle
{
    public Vector3 First    { get; }
    public Vector3 Second   { get; }
    public Vector3 Third    { get; }

    public Triangle(
        Vector3 first,
        Vector3 second,
        Vector3 third
    )
    {
        First   = first;
        Second  = second;
        Third   = third;
    }
};

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class ImplicitSurfaceRenderer : MonoBehaviour
{
    public const int ROW_COUNT          = 256;
    public const int TRIANGLE_ROW_COUNT = 16;
    
    public static int[] EDGE_TABLE = new int[ROW_COUNT]
    {
        0x0  , 0x109, 0x203, 0x30A, 0x406, 0x50F, 0x605, 0x70C,
        0x80C, 0x905, 0xA0F, 0xB06, 0xC0A, 0xD03, 0xE09, 0xF00,
        0x190, 0x99 , 0x393, 0x29A, 0x596, 0x49F, 0x795, 0x69C,
        0x99C, 0x895, 0xB9F, 0xA96, 0xD9A, 0xC93, 0xF99, 0xE90,
        0x230, 0x339, 0x33 , 0x13A, 0x636, 0x73F, 0x435, 0x53C,
        0xA3C, 0xB35, 0x83F, 0x936, 0xE3A, 0xF33, 0xC39, 0xD30,
        0x3A0, 0x2A9, 0x1A3, 0xAA , 0x7A6, 0x6AF, 0x5A5, 0x4AC,
        0xBAC, 0xAA5, 0x9AF, 0x8A6, 0xFAA, 0xEA3, 0xDA9, 0xCA0,
        0x460, 0x569, 0x663, 0x76A, 0x66 , 0x16F, 0x265, 0x36C,
        0xC6C, 0xD65, 0xE6F, 0xF66, 0x86A, 0x963, 0xA69, 0xB60,
        0x5F0, 0x4F9, 0x7F3, 0x6FA, 0x1F6, 0xFF , 0x3F5, 0x2FC,
        0xDFC, 0xCF5, 0xFFF, 0xEF6, 0x9FA, 0x8F3, 0xBF9, 0xAF0,
        0x650, 0x759, 0x453, 0x55A, 0x256, 0x35F, 0x55 , 0x15C,
        0xE5C, 0xF55, 0xC5F, 0xD56, 0xA5A, 0xB53, 0x859, 0x950,
        0x7C0, 0x6C9, 0x5C3, 0x4CA, 0x3C6, 0x2CF, 0x1C5, 0xCC ,
        0xFCC, 0xEC5, 0xDCF, 0xCC6, 0xBCA, 0xAC3, 0x9C9, 0x8C0,
        0x8C0, 0x9C9, 0xAC3, 0xBCA, 0xCC6, 0xDCF, 0xEC5, 0xFCC,
        0xCC , 0x1C5, 0x2CF, 0x3C6, 0x4CA, 0x5C3, 0x6C9, 0x7C0,
        0x950, 0x859, 0xB53, 0xA5A, 0xD56, 0xC5F, 0xF55, 0xE5C,
        0x15C, 0x55 , 0x35F, 0x256, 0x55A, 0x453, 0x759, 0x650,
        0xAF0, 0xBF9, 0x8F3, 0x9FA, 0xEF6, 0xFFF, 0xCF5, 0xDFC,
        0x2FC, 0x3F5, 0xFF , 0x1F6, 0x6FA, 0x7F3, 0x4F9, 0x5F0,
        0xB60, 0xA69, 0x963, 0x86A, 0xF66, 0xE6F, 0xD65, 0xC6C,
        0x36C, 0x265, 0x16F, 0x66 , 0x76A, 0x663, 0x569, 0x460,
        0xCA0, 0xDA9, 0xEA3, 0xFAA, 0x8A6, 0x9AF, 0xAA5, 0xBAC,
        0x4AC, 0x5A5, 0x6AF, 0x7A6, 0xAA , 0x1A3, 0x2A9, 0x3A0,
        0xD30, 0xC39, 0xF33, 0xE3A, 0x936, 0x83F, 0xB35, 0xA3C,
        0x53C, 0x435, 0x73F, 0x636, 0x13A, 0x33 , 0x339, 0x230,
        0xE90, 0xF99, 0xC93, 0xD9A, 0xA96, 0xB9F, 0x895, 0x99C,
        0x69C, 0x795, 0x49F, 0x596, 0x29A, 0x393, 0x99 , 0x190,
        0xF00, 0xE09, 0xD03, 0xC0A, 0xB06, 0xA0F, 0x905, 0x80C,
        0x70C, 0x605, 0x50F, 0x406, 0x30A, 0x203, 0x109, 0x0
    };

    public static int[][] TRIANGLE_TABLE = new int[ROW_COUNT][]
    {
        new int [TRIANGLE_ROW_COUNT] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1 },
        new int [TRIANGLE_ROW_COUNT] { 11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1 },
        new int [TRIANGLE_ROW_COUNT] { 11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1 },
        new int [TRIANGLE_ROW_COUNT] { 8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1 },
        new int [TRIANGLE_ROW_COUNT] { 6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1 },
        new int [TRIANGLE_ROW_COUNT] { 7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1 },
        new int [TRIANGLE_ROW_COUNT] { 11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1 },
        new int [TRIANGLE_ROW_COUNT] { 8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1 },
        new int [TRIANGLE_ROW_COUNT] { 7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1 },
        new int [TRIANGLE_ROW_COUNT] { 7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1 },
        new int [TRIANGLE_ROW_COUNT] { 11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1 },
        new int [TRIANGLE_ROW_COUNT] { 6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1 },
        new int [TRIANGLE_ROW_COUNT] { 5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1 },
        new int [TRIANGLE_ROW_COUNT] { 11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { 0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        new int [TRIANGLE_ROW_COUNT] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }
    };

    public VoxelGrid Grid;
    public ImplicitSurface Surface;

    public MeshFilter Filter    { get; private set; }
    public Mesh SurfaceMesh     { get; private set; }

    public List<Triangle> Triangles { get; private set; }

    private void Awake()
    {
        Filter              = GetComponent<MeshFilter>();
        SurfaceMesh         = new Mesh();
        Filter.sharedMesh   = SurfaceMesh;
        Triangles           = new List<Triangle>();
    }

    private void Start()
    {
        if (!Grid)
        {
            throw new UnityException(
                "No VoxelGrid instance supplied to ImplicitSurfaceRenderer: " + name + '!'
            );
        }

        if (!Surface)
        {
            throw new UnityException(
                "No ImplicitSurface instance supplied to ImplicitSurfaceRenderer: " + name + '!'
            );
        }
    }

    public static int GetCubeIndex(
        in Cube cube,
        ImplicitSurface surface
    )
    {
        int cube_index = 0;

        float iso_value = surface.GetIsoValue();

        if ( surface.ComputeFieldDistance(cube.BottomLeftForward) < iso_value )
        {
            cube_index |= 1;
        }

        if ( surface.ComputeFieldDistance(cube.BottomRightForward) < iso_value )
        {
            cube_index |= ( 1 << 1 );
        }

        if ( surface.ComputeFieldDistance(cube.BottomRightBack) < iso_value )
        {
            cube_index |= ( 1 << 2 );
        }

        if ( surface.ComputeFieldDistance(cube.BottomLeftBack) < iso_value )
        {
            cube_index |= ( 1 << 3 );
        }

        if ( surface.ComputeFieldDistance(cube.TopLeftForward) < iso_value )
        {
            cube_index |= ( 1 << 4 );
        }

        if ( surface.ComputeFieldDistance(cube.TopRightForward) < iso_value )
        {
            cube_index |= ( 1 << 5 );
        }

        if ( surface.ComputeFieldDistance(cube.TopRightBack) < iso_value )
        {
            cube_index |= ( 1 << 6 );
        }

        if ( surface.ComputeFieldDistance(cube.TopLeftBack) < iso_value )
        {
            cube_index |= ( 1 << 7 );
        }

        return cube_index;
    }

    public static bool HasIntersectingEdges(int cube_index)
    {
        return EDGE_TABLE[cube_index] != 0;
    }

    public static List<Edge> GetIntersectingEdges(
        in Cube cube,
        ImplicitSurface surface
    )
    {
        int cube_index = GetCubeIndex(cube, surface);

        List<Edge> intersecting_edges = new List<Edge>();

        if ( !HasIntersectingEdges(cube_index) )
        {
            return intersecting_edges;
        }

        int[] edge_indices = TRIANGLE_TABLE[cube_index];

        const int edge_index_count  = TRIANGLE_ROW_COUNT - 1;
        intersecting_edges.Capacity = edge_index_count;

        for (int i = 0; i < edge_index_count; ++i)
        {
            int edge_index = edge_indices[i];

            switch (edge_index)
            {
                case 0:
                    intersecting_edges.Add(cube.BottomForward);
                    break;
                case 1:
                    intersecting_edges.Add(cube.BottomRight);
                    break;
                case 2:
                    intersecting_edges.Add(cube.BottomBack);
                    break;
                case 3:
                    intersecting_edges.Add(cube.BottomLeft);
                    break;
                case 4:
                    intersecting_edges.Add(cube.TopForward);
                    break;
                case 5:
                    intersecting_edges.Add(cube.TopRight);
                    break;
                case 6:
                    intersecting_edges.Add(cube.TopBack);
                    break;
                case 7:
                    intersecting_edges.Add(cube.TopLeft);
                    break;
                case 8:
                    intersecting_edges.Add(cube.UpLeftForward);
                    break;
                case 9:
                    intersecting_edges.Add(cube.UpRightForward);
                    break;
                case 10:
                    intersecting_edges.Add(cube.UpRightBack);
                    break;
                case 11:
                    intersecting_edges.Add(cube.UpLeftBack);
                    break;
                default:
                    break;
            }
        }

        return intersecting_edges;
    }

    public static Vector3 GetIntersectionPoint(
        in Edge edge,
        ImplicitSurface surface
    )
    {
        float iso_value = surface.GetIsoValue();

        Vector3 first   = edge.First;
        Vector3 second  = edge.Second;

        float first_field_value     = surface.ComputeFieldDistance(first);
        float second_field_value    = surface.ComputeFieldDistance(second);

        float coefficient = ( iso_value - first_field_value ) / ( second_field_value - first_field_value );

        return first + coefficient * ( second - first );
    }

    private void LoadTriangles()
    {
        Triangles.Clear();

        SurfaceBounds surface_bounds = Surface.GetSurfaceBounds();

        Vector3 surface_max = surface_bounds.Maximum;
        Vector3 surface_min = surface_bounds.Minimum;

        int cell_count_x    = Grid.CellCountX;
        int max_x_index     = Grid.GetGridIndex(surface_max.x, cell_count_x);
        int min_x_index     = Grid.GetGridIndex(surface_min.x, cell_count_x);

        int cell_count_y    = Grid.CellCountY;
        int max_y_index     = Grid.GetGridIndex(surface_max.y, cell_count_y);
        int min_y_index     = Grid.GetGridIndex(surface_min.y, cell_count_y);

        int cell_count_z    = Grid.CellCountZ;
        int max_z_index     = Grid.GetGridIndex(surface_max.z, cell_count_z);
        int min_z_index     = Grid.GetGridIndex(surface_min.z, cell_count_z);

        List<Cube> grid_cells = Grid.Cells;

        for ( int i = min_z_index; i <= max_z_index; ++i )
        {
            for ( int j = min_y_index; j <= max_y_index; ++j )
            {
                for (int k = min_x_index; k <= max_x_index; ++k )
                {
                    List<Edge> intersecting_edges   = GetIntersectingEdges(grid_cells[Grid.GetGridIndex(k, j, i)], Surface);
                    int edge_count                  = intersecting_edges.Count;

                    if (edge_count == 0 || edge_count % 3 != 0)
                    {
                        continue;
                    }

                    for (int l = 0; l < edge_count; l += 3)
                    {
                        Triangles.Add(
                            new Triangle(
                                GetIntersectionPoint(intersecting_edges[l], Surface),
                                GetIntersectionPoint(intersecting_edges[l + 1], Surface),
                                GetIntersectionPoint(intersecting_edges[l + 2], Surface)
                            )
                        );
                    }
                }
            }
        }
    }

    private void Render()
    {
        SurfaceMesh.Clear();

        List<Vector3> vertices  = new List<Vector3>();
        List<int> triangles     = new List<int>();
        int triangle_index      = 0;

        for (int i = 0; i < Triangles.Count; ++i)
        {
            Triangle triangle = Triangles[i];

            vertices.Add(triangle.First);
            vertices.Add(triangle.Second);
            vertices.Add(triangle.Third);

            triangles.Add(triangle_index++);
            triangles.Add(triangle_index++);
            triangles.Add(triangle_index++);
        }

        SurfaceMesh.vertices    = vertices.ToArray();
        SurfaceMesh.triangles   = triangles.ToArray();
    }

    private void LateUpdate()
    {
        LoadTriangles();
        Render();
    }
};