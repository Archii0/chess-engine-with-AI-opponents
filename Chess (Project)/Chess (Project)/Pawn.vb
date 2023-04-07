Public Class Pawn
    Inherits Piece
    Implements IChessPiece

    Private FirstMoveTaken As Boolean = False
    Private FirstMoveNumber As Integer = -1
    Private Movements = New Integer() {8, 16}
    Private Captures = New Integer() {7, 9}
    Private PromotionTag As Integer = 0
    Private PromotionMoveNumber As Integer = -1
    Private EnpassentMoveNumber As Integer = -1

    Public Sub SetPromotionMoveNumber(x)
        PromotionMoveNumber = x
    End Sub
    Public Sub ResetPromotionMoveNumber()
        PromotionMoveNumber = -1
    End Sub
    Public Sub SetFirstMoveNumber(x)
        FirstMoveNumber = x
    End Sub
    Public Sub SetEnpassentMoveNumber(x)
        EnpassentMoveNumber = x
    End Sub
    Public Sub ResetEnpassentMoveNumber()
        EnpassentMoveNumber = -1
    End Sub
    Public Sub UpdateFirstMoveTaken()
        FirstMoveTaken = True
    End Sub
    Public Sub ResetFirstMoveTaken()
        FirstMoveTaken = False
    End Sub
    Public Sub ResetFirstMoveNumber()
        FirstMoveNumber = -1
    End Sub
    Public Sub SetPromotionTag(x)
        PromotionTag = x
    End Sub
    Public Sub SetIcon(x As Integer, y As Integer) Implements IChessPiece.SetIcon
        If colour = Piece.White Then
            pieceImage = My.Resources.WhitePawn
        ElseIf colour = Piece.Black Then
            pieceImage = My.Resources.BlackPawn
        End If

        With pieceRectangle
            .X = x
            .Y = y
            .Width = pieceImage.Width
            .Height = pieceImage.Height
        End With
    End Sub
    Public Function GetPieceType() As Integer Implements IChessPiece.GetPieceType
        Return Piece.Pawn
    End Function
    Public Function GetMovementArray() As Integer() Implements IChessPiece.GetMovementArray
        Return Movements
    End Function
    Public Function GetFirstMoveTaken()
        Return FirstMoveTaken
    End Function
    Public Function GetCaptureArray()
        Return Captures
    End Function
    Public Function GetPromotionTag()
        Return PromotionTag
    End Function
    Public Function GetPromotionMoveNumber()
        Return PromotionMoveNumber
    End Function
    Public Function GetFirstMoveNumber()
        Return FirstMoveNumber
    End Function
    Function GetEnpassentMoveNumber()
        Return EnpassentMoveNumber
    End Function
End Class
