import {
  Box,
  Container,
  Collapse,
  IconButton,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography
} from '@material-ui/core';
import { KeyboardArrowDown, KeyboardArrowUp } from '@material-ui/icons';
import React, { useEffect, useState } from 'react';
import filesize from 'filesize';
import moment from 'moment';
import { makeStyles, createStyles } from '@material-ui/core/styles';

const useStyles = makeStyles((theme) => ({
  title: {
    margin: theme.spacing(2)    
  },
  emptyContainer: {
    display: 'flex',
    justifyContent: 'center',
    alignContent: 'center',    
    height: '250px',
    padding: '50px'
  }
}));

const useRowStyles = makeStyles((theme) => createStyles({
  root: {
    '& > *': {
      borderBottom: 'unset',
    },
  },
  documentRow: {
    cursor: 'pointer',
    transition: 'background 0.3s',
    '&:hover': {
      background: '#009688'
    }
  }
}));


function Row(props) {
  const { row, onDocumentRowClick } = props;
  const { uploadedBy, fileCount, totalFileSize, totalAmount, totalAmountDue, documents} = row;
  const [open, setOpen] = React.useState(false);
  const classes = useRowStyles();

  return (
    <React.Fragment>
      <TableRow>
        <TableCell>
          <IconButton aria-label="expand row" size="small" onClick={() => setOpen(!open)}>
            {open ? <KeyboardArrowUp /> : <KeyboardArrowDown />}
          </IconButton>
        </TableCell>
        <TableCell component="th" scope="row">
          {uploadedBy}
        </TableCell>
        <TableCell align="right">{fileCount}</TableCell>
        <TableCell align="right">{filesize(totalFileSize)}</TableCell>
        <TableCell align="right">${totalAmount.toFixed(2)}</TableCell>
        <TableCell align="right">${totalAmountDue.toFixed(2)}</TableCell>
      </TableRow>
      <TableRow>
        <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={6}>
          <Collapse in={open} timeout="auto" unmountOnExit>
            <Box margin={1}>
              <Typography variant="h6" gutterBottom component="div">
                Documents
              </Typography>
              <Table size="small" aria-label="purchases">
                <TableHead>
                  <TableRow>
                    <TableCell>ID</TableCell>
                    <TableCell>Vendor</TableCell>
                    <TableCell>Uploaded At</TableCell>
                    <TableCell>Invoice Date</TableCell>                    
                  </TableRow>
                </TableHead>
                <TableBody>
                  {documents.map(({ id, vendorName, uploadedTimeStamp, invoiceDate }) => (
                    <TableRow key={id} className={classes.documentRow} onClick={() => onDocumentRowClick(id)}>
                      <TableCell component="th" scope="row">
                        {id}
                      </TableCell>
                      <TableCell>{vendorName}</TableCell>
                      <TableCell>{moment(uploadedTimeStamp).format('MMMM Do, YYYY, h:mm:ss A')}</TableCell>
                      <TableCell>{moment(invoiceDate).format('MMMM Do, YYYY')}</TableCell>                      
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </Box>
          </Collapse>
        </TableCell>
      </TableRow>
    </React.Fragment>
  );
}

export default function StatsTable(props) {
  const classes = useStyles();

  const { isStale, onDocumentRowClick, onFetched } = props;
  const [docStats, setStats] = useState([]);

  useEffect(() => {
    if (isStale) {
      fetch("/stats")
        .then(res => res.json())
        .then(json => {
          setStats(json);
          onFetched();
        });
    }
  }, [isStale, onFetched, setStats]); 

  return (
    <TableContainer component={Paper}>
      <Box className={classes.title}>
        <Typography variant="h5">Stats</Typography>
      </Box>
      <Table aria-label="collapsible table">
        <TableHead>
          <TableRow>
            <TableCell />
            <TableCell>Uploader</TableCell>
            <TableCell align="right">File Uploaded</TableCell>
            <TableCell align="right">Total File Size</TableCell>
            <TableCell align="right">Total Amount (USD)</TableCell>
            <TableCell align="right">Total Amount Due (USD)</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {docStats.map((row) => (
            <Row key={row.uploadedBy} row={row} onDocumentRowClick={onDocumentRowClick} />
          ))}
        </TableBody>
      </Table>
      {docStats.length === 0 && (
        <Container className={classes.emptyContainer}>
          <Typography variant="h6">No documents</Typography>
        </Container>
      )}
    </TableContainer>
  );

}