import {
  AppBar,
  Box,
  Container,
  createStyles,
  makeStyles,
  Slide,
  Toolbar,
  Typography,
  useScrollTrigger
} from '@material-ui/core';
import React from 'react';
import Home from './components/Home';

import './custom.css'

const HideOnScroll = (props) => {
  const { children, window } = props;
  const trigger = useScrollTrigger({ target: window ? window() : undefined });

  return (
    <Slide appear={false} direction="down" in={!trigger}>
      {children}
    </Slide>
  );
};

const useStyles = makeStyles((theme) =>
  createStyles({
    root: {
      flexGrow: 1
    },
    menuButton: {
      marginRight: theme.spacing(2)
    },
    title: {
      flexGrow: 1
    }
  })
);

export default function App(props) {
  const classes = useStyles();

  return (
    <Box width={1}>
      <HideOnScroll {...props} className={classes.root}>
        <AppBar position="sticky">
          <Toolbar>
            <Typography variant="h6" color="inherit" className={classes.title}>
              Invoice Documents
            </Typography>            
          </Toolbar>
        </AppBar>
      </HideOnScroll>
      <Container maxWidth="lg">
        {/* <Route exact path='/' component={Home} /> */}
        <Home />
      </Container>
    </Box>
  );
}
