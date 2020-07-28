import {
  IconButton,
  Dialog,
  DialogContent,
  DialogTitle,
  Divider,
  List,
  ListItem,
  ListItemText,
  Typography
} from '@material-ui/core';
import { Close } from '@material-ui/icons';
import { makeStyles } from '@material-ui/core/styles';
import { Skeleton } from '@material-ui/lab';
import React, { useEffect, useState } from 'react';
import filesize from 'filesize';
import moment from 'moment';

const useStyles = makeStyles((theme) => ({
  root: {
    width: '100%',
    backgroundColor: theme.palette.background.paper,
  },
  closeButton: {
    position: 'absolute',
    right: theme.spacing(1),
    top: theme.spacing(1)
  }
}));

export default function(props) {
  const classes = useStyles();

  const { id, open, onClose } = props;
  const [document, setDocument] = useState(null);
  
  const isEmpty = document === null

  useEffect(() => {
    if (id !== null) {
      fetch(`/document/${id}`)
        .then(res => res.json())
        .then(json => setDocument(json));
    }
  }, [id]); 

  const currencyFormat = !isEmpty && new Intl.NumberFormat('en-CA', { style: 'currency', currency: document.currency });

  return (
    <Dialog open={open} onClose={onClose} disableScrollLock fullWidth maxWidth="sm">
      <DialogTitle disableTypography>
        <Typography variant="h6">Document</Typography>
        <IconButton aria-label="close" className={classes.closeButton} onClick={onClose}>
          <Close />
        </IconButton>
      </DialogTitle>
      <DialogContent>
        <List className={classes.root}>
          <ListItem>
            {isEmpty ? <Skeleton/> : <ListItemText primary="ID" secondary={document.id} />}            
          </ListItem>
          <Divider />
          <ListItem>
            {isEmpty ? <Skeleton/> : <ListItemText primary="Uploader" secondary={document.uploadedBy} />}            
          </ListItem>
          <Divider />
          <ListItem>
            {isEmpty ? <Skeleton/> : <ListItemText primary="Uploaded At" secondary={moment(document.uploadTimestamp).format('MMMM Do, YYYY, h:mm:ss A')} />}            
          </ListItem>
          <Divider />
          <ListItem>
            {isEmpty ? <Skeleton/> : <ListItemText primary="File Size" secondary={filesize(document.fileSize)} />}            
          </ListItem>
          <Divider />
          <ListItem>
            {isEmpty ? <Skeleton/> : <ListItemText primary="Vendor" secondary={document.vendorName} />}            
          </ListItem>
          <Divider />
          <ListItem>
            {isEmpty ? <Skeleton/> : <ListItemText primary="Invoice Date" secondary={moment(document.invoiceDate).format('MMMM Do, YYYY')} />}            
          </ListItem>
          <Divider />
          <ListItem>
            {isEmpty ? <Skeleton/> : <ListItemText primary="Total Amount" secondary={currencyFormat.format(document.totalAmount)} />}            
          </ListItem>
          <Divider />
          <ListItem>
            {isEmpty ? <Skeleton/> : <ListItemText primary="Total Amount Due" secondary={currencyFormat.format(document.totalAmountDue)} />}            
          </ListItem>
          <Divider />
          <ListItem>
            {isEmpty ? <Skeleton/> : <ListItemText primary="Tax Amount" secondary={currencyFormat.format(document.taxAmount)} />}            
          </ListItem>
          <Divider />          
          <ListItem>
            {isEmpty ? <Skeleton/> : <ListItemText primary="Currency" secondary={document.currency} />}            
          </ListItem>
        </List>
      </DialogContent>
    </Dialog>
  );
}
