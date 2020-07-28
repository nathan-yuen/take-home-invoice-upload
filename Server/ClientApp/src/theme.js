import { red } from '@material-ui/core/colors';
import { createMuiTheme } from '@material-ui/core/styles';

// A custom theme for this app
const theme = createMuiTheme({
  palette: {
    type: 'dark',
    primary: {
      main: '#196b8c',
      light: '#5499bc',
      dark: '#00405f',
      contrastText: '#fff'
    },
    secondary: {
      main: '#c62828',
      light: '#ff5f52',
      dark: '#8e0000',
      contrastText: '#fff'
    },
    error: {
      main: red.A400
    }
  }
});

export default theme;
