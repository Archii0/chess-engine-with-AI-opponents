Public Class Bishop
    Inherits Piece
    Implements IChessPiece

    Public Movements = New Integer() {7, 9, -7, -9}

    Public Sub SetIcon(x As Integer, y As Integer) Implements IChessPiece.SetIcon
        If colour = Piece.White Then
            pieceImage = My.Resources.WhiteBishop
        ElseIf colour = Piece.Black Then
            pieceImage = My.Resources.BlackBishop
        End If
        With pieceRectangle
            .X = x
            .Y = y
            .Width = pieceImage.Width
            .Height = pieceImage.Height
        End With
    End Sub
    Public Function GetPieceType() As Integer Implements IChessPiece.GetPieceType
        Return Piece.Bishop
    End Function
    Public Function GetMovementArray() As Integer() Implements IChessPiece.GetMovementArray
        Return Movements
    End Function
End Class
