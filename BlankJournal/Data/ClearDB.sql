update tbpinfotable set DataPDF=null where LEN(datapdf)>0
update TBPInfoTable set DataWord=null where LEN(DataWord)>0
delete from TBPHistoryTable
delete from TBPCommentsTable
delete from BPJournalTable
delete from DataTable
