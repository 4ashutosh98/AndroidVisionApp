import express from 'express';
import cors from 'cors';
import multer from 'multer';
import path, { dirname } from 'path';
import { fileURLToPath } from 'url';
import fs from 'fs/promises';
import ModelClient, { isUnexpected } from '@azure-rest/ai-inference';
import { AzureKeyCredential } from '@azure/core-auth';
import dotenv from 'dotenv';
import { Buffer } from 'buffer';

dotenv.config();

// ESM __dirname polyfill
const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

// Async function to initialize the server
async function initializeServer() {
  // Ensure uploads directory exists
  const uploadsDir = path.join(__dirname, 'uploads');
  try {
    await fs.mkdir(uploadsDir, { recursive: true });
    console.log('[Backend] Uploads directory ensured:', uploadsDir);
  } catch (error) {
    console.error('[Backend] Error ensuring uploads directory:', error.message);
  }
  console.log('[Backend] uploads directory:', uploadsDir);

  // Utility function to safely delete uploaded files
  const deleteUploadedFile = async (filePath) => {
    try {
      await fs.unlink(filePath);
      console.log('[Backend] Successfully deleted uploaded file:', filePath);
    } catch (error) {
      console.error('[Backend] Failed to delete uploaded file:', filePath, 'Error:', error.message);
    }
  };

  const app = express();
  app.use(cors());
  // Serve uploaded images statically
  app.use('/uploads', express.static(path.join(__dirname, 'uploads')));

  // Health check endpoint
  app.get('/health', (_req, res) => {
    res.json({ status: 'ok' });
  });
  // Configure disk storage for uploads
  const storage = multer.diskStorage({
    destination: path.join(__dirname, 'uploads'),
    filename: (_req, file, cb) => {
      const uniqueName = `${Date.now()}_${file.originalname}`;
      cb(null, uniqueName);
    },
  });
  const upload = multer({ storage });

  // GitHub AI multimodal inference only
  const ghEndpoint = 'https://models.github.ai/inference';
  const ghToken = process.env.GITHUB_TOKEN;
  const ghModel = 'meta/Llama-3.2-11B-Vision-Instruct';
  const ghClient = ModelClient(ghEndpoint, new AzureKeyCredential(ghToken));
  console.log('GITHUB_TOKEN=', !!process.env.GITHUB_TOKEN);

  // Multimodal LLM endpoint: pass public URL to model
  app.post('/vision', upload.single('image'), async (req, res) => {
  console.log('[Backend] /vision endpoint hit from', req.ip);
  console.log('[Backend] multer file info:', req.file);
  if (req.file) {
    console.log('[Backend] File saved at:', req.file.destination + '/' + req.file.filename);
  } else {
    console.warn('[Backend] No file received by multer!');
    return res.status(400).json({ error: 'No image file received' });
  }
  
  try {
    const filename = req.file.filename;
    const imageUrl = `${req.protocol}://${req.get('host')}/uploads/${filename}`;
    console.log('[Backend] Uploaded image available at:', imageUrl);
    
    // Fetch image and convert to base64 for the API
    const image_response = await fetch(imageUrl, {headers: {"User-Agent": "Mozilla/5.0"}});
    const image_data = await image_response.arrayBuffer();
    console.log('[Backend] Fetched image data from URL:', imageUrl);
    const image_data_base64 = Buffer.from(image_data).toString('base64');
    const data_url = `data:image/jpeg;base64,${image_data_base64}`;
    
    // Correct message structure for GitHub AI API
    const messages = [
      {
        role: 'system',
        content: 'You are a helpful assistant that can read and extract text from images using advanced vision understanding.'
      },
      {
        role: 'user',
        content: [
          {
            type: 'text',
            text: 'Please extract all the text visible in this image. Only return the text content, nothing else.'
          },
          {
            type: 'image_url',
            image_url: {
              url: data_url
            }
          }
        ]
      }
    ];
    
    console.log('[Backend] Sending request to GitHub API, model:', ghModel);
    console.log('[Backend] Message structure:', JSON.stringify(messages, null, 2));
    
    const response = await ghClient.path('/chat/completions').post({
      body: {
        messages,
        model: ghModel,
        temperature: 0.1,
        max_tokens: 1000
      }
    });
    
    console.log('[Backend] GitHub API response status:', response.status);
      if (isUnexpected(response)) {
      console.error('[Backend] Unexpected response:', response);
      console.error('[Backend] Response body:', response.body);
      throw new Error(response.body?.error?.message || 'Unknown API error');
    }
    
    const output = response.body.choices[0].message.content;
    console.log('[Backend] Extracted LLM output:', output);
    res.json({ result: output, imageUrl });
    console.log('[Backend] Response sent to frontend');
    
    // Clean up the uploaded file after successful processing
    const filePath = path.join(req.file.destination, req.file.filename);
    await deleteUploadedFile(filePath);
  } catch (e) {
    console.error('[Backend] Vision LLM error:', e);
    console.error('[Backend] Error details:', e.message);
    res.status(500).json({ error: e.message || 'Vision LLM processing failed' });
    
    // Clean up the uploaded file even if processing failed
    if (req.file) {
      const filePath = path.join(req.file.destination, req.file.filename);
      await deleteUploadedFile(filePath);
    }
  }
});



  const port = process.env.PORT || 3000;
  app.listen(port, () => console.log(`Backend listening on port ${port}`));
}

// Initialize the server
initializeServer().catch(error => {
  console.error('[Backend] Failed to initialize server:', error);
  process.exit(1);
});
