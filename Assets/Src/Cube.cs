using UnityEngine;

readonly public struct Edge
{
    public Vector3 First    { get; }
    public Vector3 Second   { get; }

    public Edge(
        Vector3 first,
        Vector3 second
    )
    {
        First   = first;
        Second  = second;
    }
};

readonly public struct Cube
{
    // Edges:

        // Bottom:

    public Edge BottomForward   { get; }
    public Edge BottomRight     { get; }
    public Edge BottomBack      { get; }
    public Edge BottomLeft      { get; }

        // Top:

    public Edge TopForward  { get; }
    public Edge TopRight    { get; }
    public Edge TopBack     { get; }
    public Edge TopLeft     { get; }

        // Up:

    public Edge UpLeftForward   { get; }
    public Edge UpRightForward  { get; }
    public Edge UpRightBack     { get; }
    public Edge UpLeftBack      { get; }

    // Face vertices:

        // Bottom:

    public Vector3 BottomLeftForward    { get; }
    public Vector3 BottomRightForward   { get; }
    public Vector3 BottomRightBack      { get; }
    public Vector3 BottomLeftBack       { get; }

        // Top:
    
    public Vector3 TopLeftForward   { get; }
    public Vector3 TopRightForward  { get; }
    public Vector3 TopRightBack     { get; }
    public Vector3 TopLeftBack      { get; }

    public Cube(
        float   size,
        Vector3 bottom_left_back
    )
    {
        BottomLeftForward   = bottom_left_back + new Vector3(0.0f, 0.0f, size);
        BottomRightForward  = BottomLeftForward + new Vector3(size, 0.0f);
        BottomRightBack     = bottom_left_back + new Vector3(size, 0.0f);
        BottomLeftBack      = bottom_left_back;

        TopLeftForward  = BottomLeftForward + new Vector3(0.0f, size);
        TopRightForward = BottomRightForward + new Vector3(0.0f, size);
        TopRightBack    = BottomRightBack + new Vector3(0.0f, size);
        TopLeftBack     = bottom_left_back + new Vector3(0.0f, size);

        BottomForward   = new Edge(BottomLeftForward, BottomRightForward);
        BottomRight     = new Edge(BottomRightForward, BottomRightBack);
        BottomBack      = new Edge(BottomRightBack, bottom_left_back);
        BottomLeft      = new Edge(bottom_left_back, BottomLeftForward);

        TopForward  = new Edge(TopLeftForward, TopRightForward);
        TopRight    = new Edge(TopRightForward, TopRightBack);
        TopBack     = new Edge(TopRightBack, TopLeftBack);
        TopLeft     = new Edge(TopLeftBack, TopLeftForward);

        UpLeftForward   = new Edge(TopLeftForward, BottomLeftForward);
        UpRightForward  = new Edge(TopRightForward, BottomRightForward);
        UpRightBack     = new Edge(TopRightBack, BottomRightBack);
        UpLeftBack      = new Edge(TopLeftBack, bottom_left_back);
    }
};