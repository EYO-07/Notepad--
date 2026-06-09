//#ifndef CODEX_DarkQtScintilla_H
//#define CODEX_DarkQtScintilla_H
#pragma once
#include "CODEX_DarkQt.h"
#include <Qsci/qsciscintillabase.h>
#include <Qsci/qsciscintilla.h>
#include <Qsci/qscilexer.h>
#include <Qsci/qsciapis.h>
// --
#include <Qsci/qscilexercpp.h>
#include <Qsci/qscilexercsharp.h>
#include <Qsci/qscilexerpython.h>
#include <Qsci/qscilexerjava.h>
#include <Qsci/qscilexerjavascript.h>
#include <Qsci/qscilexerhtml.h>
#include <Qsci/qscilexerxml.h>
#include <Qsci/qscilexercss.h>
#include <Qsci/qscilexerjson.h>
#include <Qsci/qscilexerbash.h>
#include <Qsci/qscilexerbatch.h>
#include <Qsci/qscilexersql.h>
#include <Qsci/qscilexerruby.h>
#include <Qsci/qscilexerlua.h>
#include <Qsci/qscilexermarkdown.h>
#include <Qsci/qscilexeryaml.h>
#include <Qsci/qscilexerproperties.h>
#include <Qsci/qscilexerperl.h>
#include <Qsci/qscilexercmake.h>
#include <Qsci/qscilexercoffeescript.h>
#include <Qsci/qscilexerfortran77.h>
#include <Qsci/qscilexerfortran.h>
#include <Qsci/qscilexermatlab.h>
#include <Qsci/qscilexertex.h>
// --
#include <QDebug>
#include <QSharedMemory>
#include <QColor>
#include <QObject>
#include <QMessageBox>
#include <QSplitter>
#include <QRegularExpression>
#include <QStringList>
#include <algorithm>

namespace CodexIncantation {
    // functions
    QsciScintilla* newDarkScintilla(QWidget *parent = nullptr, QString fileName = "dummy.cpp");
    void resetScintilla(QsciScintilla* editor, QString fileName); // ?
    void toggleCurrentFold(QsciScintilla *editor);
    bool setLexer(QsciScintilla* editor, QString fileName);
    bool setLexerFolding(QsciScintilla* editor, QString fileName);
    void setAutocompletion(QsciScintilla* editor);
    void updateAutocompletion_Range(QsciScintilla* editor);
    void updateAutocompletion_Full(QsciScintilla* editor);
    QString getCurrentTypingWord(QsciScintilla* editor);
    void applyIndicatorsFromTextDirectives(QsciScintilla* editor);
    void findNext(QsciScintilla* editor, QString text);
    void findPrevious(QsciScintilla* editor, QString text);
    QString getSearchStringFromDocDirective(QsciScintilla* editor);
    template<typename T> void applyLexerColorsToList(T lexer, QList<int> list, QColor fore, QColor back) {
        if (!lexer) return;
        for(const auto& item: list) {
            lexer->setColor(fore, item);
            lexer->setPaper(back, item);
        }
    }
    void hideMargin(QsciScintilla* editor);
    void setMargin(QsciScintilla* editor);
    void toggleCommentLine(QsciScintilla* editor, QString fileName);
    // functions || events
    void onTextChange(QsciScintilla* editor, std::function<void(QsciScintilla*)> logic);
    // -- 
    namespace TabbedSplitView {
        QsciScintilla* addLeftTab_Scintilla(QSplitter* view, QString name);
        QsciScintilla* addRightTab_Scintilla(QSplitter* view, QString name);
        QsciScintilla* addLeftTab_Scintilla(QSplitter* view, QString name, QString content);
        QsciScintilla* addRightTab_Scintilla(QSplitter* view, QString name, QString content);
        QsciScintilla* dialogScintillaTabLoad(QTabWidget* tabs);
        QsciScintilla* loadScintillaFromFilename(QTabWidget* tabs, QString fileName);
        bool isFileAlreadyOpened(QSplitter* splitter, QString absFilepath);
        bool isFileAlreadyOpened(QTabWidget* currentTabs, QString absFilepath);
        QString getScintillaFullFileName(QTabWidget* tabs, int tabIndex);
    }
    // classes 
    class FileRegistry {
    private:
        QSharedMemory shm;
        int memory_size = 200 * 1024; // 200 KB
    public:
        FileRegistry(const QString &key) : shm(key) {
            if (!shm.attach()) {
                if (!shm.create(memory_size)) {
                    qWarning() << "Unable to create or attach shared memory segment";
                }
            }
        }
        bool isFileOpen(const QString &path) {
            if (!shm.isAttached()) return false;
            shm.lock();
            QString content = QString::fromUtf8(static_cast<char*>(shm.data()));
            shm.unlock();
            return content.split("\n", Qt::SkipEmptyParts).contains(path);
        }
        bool registerFile(const QString &path) {
            if (!shm.isAttached()) return false;
            shm.lock();
            QString content = QString::fromUtf8(static_cast<char*>(shm.data()));
            QStringList files = content.split("\n", Qt::SkipEmptyParts);
            if (files.contains(path)) {
                shm.unlock();
                return false; // already open
            }
            files.append(path);
            QByteArray newData = files.join("\n").toUtf8();
            if (newData.size() > shm.size()) { // Safety check
                qWarning() << "Shared memory full, cannot register more files";
                shm.unlock();
                return false;
            }
            memset(shm.data(), 0, shm.size());
            memcpy(shm.data(), newData.constData(), newData.size());
            shm.unlock();
            return true;
        }
        void unregisterFile(const QString &path) {
            if (!shm.isAttached()) return;
            shm.lock();
            QString content = QString::fromUtf8(static_cast<char*>(shm.data()));
            QStringList files = content.split("\n", Qt::SkipEmptyParts);
            files.removeAll(path);
            QByteArray newData = files.join("\n").toUtf8();
            memset(shm.data(), 0, shm.size());
            memcpy(shm.data(), newData.constData(), newData.size());
            shm.unlock();
        }
    };

}

//#endif // CODEX_DarkQtScintilla_H

/*

if ( QMessageBox::No == QMessageBox::question(
    editor, 
    "Confirmation", 
    QString("Are you sure to open this file (%1) in text editor? Some files are not supposed to be opened this way, be cautious!").arg(fileName), 
    QMessageBox::Yes | QMessageBox::No
)){ 
    return true; 
}

*/



