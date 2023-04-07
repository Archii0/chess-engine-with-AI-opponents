Public MustInherit Class Piece
    Implements IDisposable

    Public Const None As Integer = 0
    Public Const Pawn As Integer = 1
    Public Const Knight As Integer = 2
    Public Const Bishop As Integer = 3
    Public Const Rook As Integer = 4
    Public Const Queen As Integer = 5
    Public Const King As Integer = 6

    Public Const White As Integer = 8
    Public Const Black As Integer = 16

    Protected colour As Integer
    Protected squareNumber As Integer

    Protected pieceIndex As Integer
    Protected rank As Integer
    Protected file As Integer

    Protected pieceImage As Bitmap
    Public pieceRectangle As Rectangle
    Private disposedValue As Boolean
    Public Sub SetImage(image)
        pieceImage = image
        pieceRectangle.Width = 68
        pieceRectangle.Height = 68
    End Sub
    Public Sub SetPieceIndex(index)
        pieceIndex = index
    End Sub
    Public Sub SetPieceColour(x)
        colour = x
    End Sub

    Public Sub SetRank(x)
        rank = x
    End Sub
    Public Sub SetFile(x)
        file = x
    End Sub
    Public Sub SetPosition(x, y, width, height)
        pieceRectangle.X = x
        pieceRectangle.Y = y

        pieceRectangle.Width = width
        pieceRectangle.Height = height
    End Sub
    Public Sub SetSquareNumber(number)
        squareNumber = number
        file = number Mod 8
        rank = number \ 8
    End Sub
    Public Sub SetSquareNumber(inputFile, inputRank)
        file = inputFile
        rank = inputRank
        squareNumber = (inputRank * 8) + inputFile

    End Sub
    Public Function GetPieceIndex()
        Return pieceIndex
    End Function
    Public Function GetPieceColour()
        Return colour
    End Function
    Public Function GetRank()
        Return rank
    End Function
    Public Function GetFile()
        Return file
    End Function
    Public Function GetXPosition()
        Return pieceRectangle.X
    End Function
    Public Function GetYPosition()
        Return pieceRectangle.Y
    End Function
    Public Function GetImage()
        Return pieceImage
    End Function
    Public Function GetPieceRectangle()
        Return pieceRectangle
    End Function
    Public Function GetSquareNumber()
        Return squareNumber
    End Function
    Public Function GetTargetSquare()
        Return TargetBoardSquare
    End Function
    Public Function GetTargetFile()
        Return targetFile
    End Function
    Public Function GetMoveCount()
        Return moveCount
    End Function
    Public Function GetTargetRank()
        Return targetRank
    End Function

#Region "IDisposable Support"
    Sub ResetDisposeValue()
        disposedValue = False
    End Sub
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            pieceRectangle = Nothing
            disposedValue = True
        End If
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        'Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(ChessGame)
    End Sub
#End Region
End Class
