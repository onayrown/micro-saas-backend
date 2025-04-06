import React, { useState, useRef } from 'react';
import {
  Box,
  Button,
  Typography,
  Grid,
  IconButton,
  CircularProgress,
  Paper,
  Alert,
  AlertTitle,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction
} from '@mui/material';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import DeleteIcon from '@mui/icons-material/Delete';
import ImageIcon from '@mui/icons-material/Image';
import VideoLibraryIcon from '@mui/icons-material/VideoLibrary';
import InsertDriveFileIcon from '@mui/icons-material/InsertDriveFile';

interface MediaUploadProps {
  onMediaSelected: (files: File[]) => void;
  onMediaRemoved: (index: number) => void;
  selectedFiles: File[];
  uploadedUrls: string[];
  isUploading?: boolean;
  error?: string | null;
  maxFiles?: number;
  acceptedFileTypes?: string;
}

const MediaUpload: React.FC<MediaUploadProps> = ({
  onMediaSelected,
  onMediaRemoved,
  selectedFiles,
  uploadedUrls,
  isUploading = false,
  error = null,
  maxFiles = 10,
  acceptedFileTypes = 'image/*,video/*'
}) => {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [dragActive, setDragActive] = useState<boolean>(false);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      const newFiles = Array.from(e.target.files);
      const totalFiles = selectedFiles.length + newFiles.length;
      
      if (totalFiles > maxFiles) {
        alert(`Você pode selecionar no máximo ${maxFiles} arquivos.`);
        return;
      }
      
      onMediaSelected(newFiles);
    }
  };

  const handleDrag = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.stopPropagation();
    
    if (e.type === 'dragenter' || e.type === 'dragover') {
      setDragActive(true);
    } else if (e.type === 'dragleave') {
      setDragActive(false);
    }
  };

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.stopPropagation();
    setDragActive(false);
    
    if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
      const newFiles = Array.from(e.dataTransfer.files);
      const totalFiles = selectedFiles.length + newFiles.length;
      
      if (totalFiles > maxFiles) {
        alert(`Você pode selecionar no máximo ${maxFiles} arquivos.`);
        return;
      }
      
      onMediaSelected(newFiles);
    }
  };

  const handleButtonClick = () => {
    fileInputRef.current?.click();
  };

  const getFileIcon = (file: File) => {
    if (file.type.startsWith('image/')) {
      return <ImageIcon />;
    } else if (file.type.startsWith('video/')) {
      return <VideoLibraryIcon />;
    } else {
      return <InsertDriveFileIcon />;
    }
  };

  const getFilePreview = (file: File) => {
    if (file.type.startsWith('image/')) {
      return URL.createObjectURL(file);
    }
    return null;
  };

  const renderFileList = () => {
    if (selectedFiles.length === 0 && uploadedUrls.length === 0) {
      return null;
    }

    return (
      <Paper variant="outlined" sx={{ mt: 2, p: 2 }}>
        <Typography variant="subtitle1" gutterBottom>
          Arquivos selecionados ({selectedFiles.length + uploadedUrls.length})
        </Typography>
        <List dense>
          {selectedFiles.map((file, index) => (
            <ListItem key={`file-${index}`}>
              <Box sx={{ mr: 2 }}>
                {getFileIcon(file)}
              </Box>
              <ListItemText 
                primary={file.name} 
                secondary={`${(file.size / 1024 / 1024).toFixed(2)} MB`} 
              />
              <ListItemSecondaryAction>
                <IconButton 
                  edge="end" 
                  aria-label="delete" 
                  onClick={() => onMediaRemoved(index)}
                  disabled={isUploading}
                >
                  <DeleteIcon />
                </IconButton>
              </ListItemSecondaryAction>
            </ListItem>
          ))}
          {uploadedUrls.map((url, index) => (
            <ListItem key={`url-${index}`}>
              <Box sx={{ mr: 2 }}>
                <ImageIcon />
              </Box>
              <ListItemText 
                primary={url.split('/').pop()} 
                secondary="Já enviado" 
              />
            </ListItem>
          ))}
        </List>
      </Paper>
    );
  };

  return (
    <Box sx={{ width: '100%' }}>
      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          <AlertTitle>Erro</AlertTitle>
          {error}
        </Alert>
      )}
      
      <Box
        sx={{
          border: '2px dashed',
          borderColor: dragActive ? 'primary.main' : 'grey.400',
          borderRadius: 1,
          p: 3,
          textAlign: 'center',
          backgroundColor: dragActive ? 'action.hover' : 'background.paper',
          transition: 'all 0.2s ease',
          cursor: 'pointer'
        }}
        onDragEnter={handleDrag}
        onDragLeave={handleDrag}
        onDragOver={handleDrag}
        onDrop={handleDrop}
        onClick={handleButtonClick}
      >
        <input
          type="file"
          ref={fileInputRef}
          style={{ display: 'none' }}
          onChange={handleFileChange}
          multiple
          accept={acceptedFileTypes}
          disabled={isUploading}
        />
        
        <CloudUploadIcon sx={{ fontSize: 48, color: 'primary.main', mb: 2 }} />
        
        <Typography variant="h6" gutterBottom>
          Arraste e solte arquivos aqui
        </Typography>
        
        <Typography variant="body2" color="textSecondary" gutterBottom>
          ou
        </Typography>
        
        <Button 
          variant="contained" 
          component="span"
          disabled={isUploading}
        >
          Selecionar Arquivos
        </Button>
        
        <Typography variant="caption" display="block" sx={{ mt: 1 }}>
          Formatos aceitos: JPG, PNG, GIF, MP4, MOV (máx. {maxFiles} arquivos)
        </Typography>
      </Box>
      
      {isUploading && (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
          <CircularProgress size={24} sx={{ mr: 1 }} />
          <Typography variant="body2">Enviando arquivos...</Typography>
        </Box>
      )}
      
      {renderFileList()}
    </Box>
  );
};

export default MediaUpload;
