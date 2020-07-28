import {
  Box,
  Button,
  Collapse,
  Container,
  FormControl,
  Typography,
  TextField,
  Input,
  InputLabel,
  Paper,
  LinearProgress
} from '@material-ui/core';
import { Alert } from '@material-ui/lab';
import { makeStyles } from '@material-ui/core/styles';
import React, { useState } from 'react';

const useStyles = makeStyles((theme) => ({
  root: {
    marginTop: theme.spacing(4),
    marginBottom: theme.spacing(4),
    padding: theme.spacing(2)
  },
  fileInput: {
    display: "none"
  },
  inputWrapper: {
    marginTop: theme.spacing(2),
    marginBottom: theme.spacing(2)
  }
}));

export default function DocumentUpload(props) {
  const classes = useStyles();

  const { onSuccess } = props;

  const [email, setEmail] = useState('');
  const [file, setFile] = useState(null);
  const [uploading, setUploading] = useState(false);
  const [success, setSuccess] = useState(null);
  const [error, setError] = useState(null);

  const emptyInput = email === '' || file === null;

  const onInputChange = (event) => {
    if (event.target.files.length > 0) {
      setFile(event.target.files[0]);
    } else {
      setFile(null);
    }  
  }

  const onEmailChange = (event) => {
    setEmail(event.target.value);
  };

  const onSuccessDismiss = () => setError(null);

  const onErrorDismiss = () => setError(null);

  const onUploadClick = () => {
    const formData = new FormData();
    formData.append("email", email);
    formData.append("file", file);
    setUploading(true);

    fetch('/upload', {
      method: 'post',
      body: formData,
    }).then(res => res.json())
    .then(data => {
      setUploading(false);
      const { success, message} = data;

      if (success) {
        onSuccess();
        setSuccess(setSuccess);
        setTimeout(onSuccessDismiss, 2000);
      } else {
        setError(message);
        setTimeout(onErrorDismiss, 4000);
      }
    });    
  };

  return (
    <Paper className={classes.root}>
      <Typography variant="h5">Upload document</Typography>
      <Container maxWidth="sm" >
        <Box className={classes.inputWrapper}>
          <TextField 
            label="Email" 
            value={email} 
            onChange={onEmailChange} 
            disabled={uploading} 
            fullWidth 
            InputLabelProps={{ shrink: true }}
          /> 
        </Box>
        <Box className={classes.inputWrapper}>
          <FormControl fullWidth disabled={uploading}>
            <InputLabel htmlFor="my-input" shrink>File</InputLabel>
            <Input type="file" inputProps={{ accept: "application/pdf" }} onChange={onInputChange} />
          </FormControl>
        </Box>      
        <Box className={classes.inputWrapper}>
          {uploading && <LinearProgress />}
          <Button variant="contained" color="primary" disabled={emptyInput || uploading} onClick={onUploadClick}>Upload</Button>
        </Box>
        <Box className={classes.inputWrapper}>
          <Collapse in={success || error !== null}>
            {success && <Alert severity="success" onClose={onSuccessDismiss}>Sucessfully uploaded!</Alert> }
            {error && <Alert severity="error" onClose={onErrorDismiss}>{error}</Alert> }            
          </Collapse>
        </Box>
      </Container>
    </Paper>
  );
}