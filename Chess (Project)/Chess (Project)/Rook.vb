Public Class Rook
    Inherits Piece
    Implements IChessPiece

    Private FirstMoveTaken As Boolean = False
    Private FirstMoveNumber As Integer = -1
    Public Movements = New Integer() {1, 8, -1, -8}

    Public Sub SetFirstMoveNumber(x)
        FirstMoveNumber = x
    End Sub
    Public Sub ResetFirstMoveNumber()
        FirstMoveNumber = -1
    End Sub
    Public Sub UpdateFirstMoveTaken()
        FirstMoveTaken = True
    End Sub
    Public Sub ResetFirstMoveTaken()
        FirstMoveTaken = False
    End Sub
    Public Sub SetIcon(x As Integer, y As Integer) Implements IChessPiece.SetIcon
        If colour = Piece.White Then
            pieceImage = My.Resources.WhiteRook
        ElseIf colour = Piece.Black Then
            pieceImage = My.Resources.BlackRook
        End If

        With pieceRectangle
            .X = x
            .Y = y
            .Width = pieceImage.Width
            .Height = pieceImage.Height
        End With
    End Sub
    Public Function GetPieceType() As Integer Implements IChessPiece.GetPieceType
        Return Piece.Rook
    End Function
    Public Function GetFirstMoveNumber()
        Return FirstMoveNumber
    End Function
    Public Function GetMovementArray() As Integer() Implements IChessPiece.GetMovementArray
        Return Movements
    End Function
    Public Function GetFirstMoveTaken()
        Return FirstMoveTaken
    End Function
End Class
