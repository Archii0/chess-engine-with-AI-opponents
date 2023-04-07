Public Class BoardSquare
    Private SquareColour As Color
    Private pieceType As Integer = Piece.None
    Private pieceColour As Integer = Piece.None
    Private pieceIndex As Integer = Piece.None

    Private file As Integer
    Private rank As Integer

    Private north As Integer
    Private east As Integer
    Private west As Integer
    Private south As Integer

    Private northEast As Integer
    Private southEast As Integer
    Private northWest As Integer
    Private southWest As Integer

    Public width As Integer = 90
    Private square As New Rectangle
    Public Sub SetFileAndRank(fileNum, rankNum)
        rank = rankNum
        file = fileNum
    End Sub
    Public Sub SetSquareColour(colour)
        SquareColour = colour
    End Sub
    Public Function GetSquareColour()
        Return SquareColour
    End Function
    Public Sub SetPieceDetails(piece, colour, index)
        pieceType = piece
        pieceColour = colour
        pieceIndex = index
    End Sub
    Public Sub ResetPieceDetails()
        pieceType = Piece.None
        pieceColour = Piece.None
        pieceIndex = Piece.None
    End Sub
    Public Sub SetDirections(N, E, S, W)
        north = N
        east = E
        south = S
        west = W

        northEast = Math.Min(N, E)
        southEast = Math.Min(S, E)
        northWest = Math.Min(N, W)
        southWest = Math.Min(S, W)
    End Sub
    Public Sub SetRectangleDetails(x, y)
        square.X = x
        square.Y = y

        square.Width = 90
        square.Height = 90
    End Sub
    Public Sub SetPieceColour(x)
        pieceColour = x
    End Sub
    Public Sub SetPieceType(x)
        pieceType = x
    End Sub
    Public Function GetRectangle() As Rectangle
        Return square
    End Function
    Public Function GetRectangleX()
        Return square.X
    End Function
    Public Function GetRectangleY()
        Return square.Y
    End Function
    Public Function GetPieceType()
        Return pieceType
    End Function
    Public Function GetPieceColour()
        Return pieceColour
    End Function
    Public Function GetPieceIndex()
        Return pieceIndex
    End Function
    Public Function GetNorth()
        Return north
    End Function
    Public Function GetEast()
        Return east
    End Function
    Public Function GetSouth()
        Return south
    End Function
    Public Function GetWest()
        Return west
    End Function
    Public Function GetNorthEast()
        Return northEast
    End Function
    Public Function GetSouthEast()
        Return southEast
    End Function
    Public Function GetNorthWest()
        Return northWest
    End Function
    Public Function GetSouthWest()
        Return southWest
    End Function
    Public Function GetRank()
        Return rank
    End Function
    Public Function GetFile()
        Return file
    End Function
End Class
