
from PyQt5.QtWidgets import *
from PyQt5.QtCore    import *
from PyQt5.QtGui     import *

class myWindow(QWidget):
    def __init__(self, parent=None):
        super(myWindow, self).__init__(parent)
        l = QHBoxLayout()
        l.addWidget(Connecter_widget())
        l.addWidget(Connecter_widget())
        l.addWidget(Connecter_widget())
        l.addWidget(Connecter_widget())
        l.setSpacing(0)
        self.setLayout(l)


class Connecter_widget(QWidget):
    def __init__(self, parent=None):
        super(Connecter_widget, self).__init__(parent)
        self.active = False

        self.active_color = {
            "border" : "#c2c2c2",
            "grid"   : "#c2c2c2",
            "fill"   : "#c2c2c2"
        }

        self.deactive_color = {
            "border" : "#c2c2c2",
            "grid"   : "#c2c2c2",
            "fill"   : "#c2c2c2"
        }

        self.hover_color = {
            "border" : "#c2c2c2",
            "grid"   : "#c2c2c2",
            "fill"   : "#ff0032"
        }

        self.color_dict = self.deactive_color

    def paintEvent(self, event):
        qp = QPainter()
        qp.begin(self)
        qp.setRenderHint(QPainter.Antialiasing, True)
        self.draw_grid(event, qp)
        self.draw_connector(event, qp)
        qp.end()

    def draw_connector(self, event, qp):
        w, h = self.size().width(), self.size().height()
        qp.setPen(QColor(self.color_dict["border"]))
        qp.setBrush(QColor(self.color_dict["fill"]))
        qp.drawEllipse (QPointF(w/2.0, h/2.0), 3, 3)

    def draw_grid(self, event, qp):
        w, h = self.size().width(), self.size().height()
        qp.setPen(QColor(self.color_dict["border"]))
        qp.setBrush(QColor(self.color_dict["fill"]))
        qp.drawLine(QPointF(0, h/2.0), QPointF(w, h/2.0))

    def enterEvent(self, event):
        self.setProperty('hovered', True)
        self.color_dict = self.hover_color
        self.update()

    def leaveEvent(self, event):
        self.setProperty('hovered', False)
        self.color_dict = self.deactive_color
        self.update()

    def mousePressEvent(self,event):
        if event.button() == Qt.LeftButton:
            self.active = not self.active

class Input_Channel(object):
       def __init__(self, parent=None):
        super(Input_Channel, self).__init__(parent) 


if __name__ == "__main__":
    import sys

    app  = QApplication(sys.argv)
    main = myWindow()
    main.show()
    main.resize(400, 600)
    sys.exit(app.exec_())